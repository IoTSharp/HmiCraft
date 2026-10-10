import fs from 'node:fs';
import path from 'node:path';
import crypto from 'node:crypto';

const root = process.argv[2];
const output = process.argv[3];
if (!root || !output || !path.isAbsolute(root) || !path.isAbsolute(output)) throw new Error('Two absolute paths required');
const critical = process.argv.slice(4);
if (critical.length > 16) throw new Error('At most 16 explicitly selected PE files are allowed');
for (const relative of critical) {
  const resolved = path.resolve(root, relative);
  if (!relative || path.isAbsolute(relative) || !resolved.startsWith(path.resolve(root) + path.sep)) throw new Error('Selected PE file must be relative to the source directory');
}
const started = Date.now();
const maxFiles = 25000, maxDirectories = 2000, maxDepth = 10, timeoutMs = 60000;
const entries = [], queue = [{directory:root, depth:0}], skipped = [];
let directories = 0;
for (let cursor = 0; cursor < queue.length && cursor < maxDirectories; cursor++) {
  if (Date.now()-started > timeoutMs) throw new Error('Inventory deadline exceeded');
  const {directory, depth} = queue[cursor]; directories++;
  for (const item of fs.readdirSync(directory, {withFileTypes:true})) {
    if (entries.length >= maxFiles || Date.now()-started > timeoutMs) throw new Error('Inventory bound exceeded');
    const absolute = path.join(directory,item.name), relative = path.relative(root,absolute).replaceAll('\\','/');
    if (item.isSymbolicLink()) { skipped.push({path:relative,reason:'link'}); continue; }
    if (item.isDirectory()) {
      if (depth < maxDepth) queue.push({directory:absolute,depth:depth+1});
      else skipped.push({path:relative,reason:'depth'});
    } else if (item.isFile()) {
      const stat = fs.statSync(absolute);
      entries.push({path:relative,size:stat.size,extension:path.extname(item.name).toLowerCase()});
    }
  }
}
if (queue.length > maxDirectories) throw new Error('Directory bound exceeded');
const countBy = field => Object.fromEntries([...Map.groupBy(entries,field)].map(([key,value])=>[key,{files:value.length,bytes:value.reduce((n,x)=>n+x.size,0)}]));
function inspectPe(relative) {
  const file = path.join(root,relative), b = fs.readFileSync(file), pe = b.readUInt32LE(0x3c), optional = pe+24;
  if (b.toString('ascii',0,2)!=='MZ' || b.toString('ascii',pe,pe+4)!=='PE\0\0') throw new Error('Invalid PE');
  const is64=b.readUInt16LE(optional)===0x20b, dir=optional+(is64?112:96), sectionStart=optional+b.readUInt16LE(pe+20), sections=[];
  const readZ = offset => { if(offset<0 || offset>=b.length) return '';const end=b.indexOf(0,offset);return b.toString('utf8',offset,Math.min(end<0?b.length:end,offset+1024)); };
  for (let i=0;i<Math.min(b.readUInt16LE(pe+6),96);i++){const s=sectionStart+i*40;sections.push({name:b.toString('ascii',s,s+8).replaceAll('\0',''),virtualSize:b.readUInt32LE(s+8),rva:b.readUInt32LE(s+12),rawSize:b.readUInt32LE(s+16),offset:b.readUInt32LE(s+20)});}
  const at = rva => {const s=sections.find(s=>rva>=s.rva && rva<s.rva+Math.max(s.virtualSize,s.rawSize));return s?s.offset+rva-s.rva:rva;};
  const imports=[], importRva=b.readUInt32LE(dir+8), exportRva=b.readUInt32LE(dir), exports=[];
  if(importRva) for(let i=0;i<128;i++){const p=at(importRva)+i*20;if(p+20>b.length)break;const name=b.readUInt32LE(p+12);if(!name)break;const symbols=[];const thunk=b.readUInt32LE(p)||b.readUInt32LE(p+16);for(let j=0;j<4096;j++){const t=at(thunk)+j*(is64?8:4);if(t+(is64?8:4)>b.length)break;const v=is64?b.readBigUInt64LE(t):BigInt(b.readUInt32LE(t));if(!v)break;const mask=is64?0x8000000000000000n:0x80000000n;symbols.push(v&mask?'#'+String(v&0xffffn):readZ(at(Number(v))+2));}imports.push({library:readZ(at(name)),symbols});}
  if(exportRva){const e=at(exportRva),names=b.readUInt32LE(e+24),table=at(b.readUInt32LE(e+32));for(let i=0;i<Math.min(names,20000);i++)exports.push(readZ(at(b.readUInt32LE(table+i*4))));}
  const candidates=[];const expressions=[/\.(hs|knr|laz|sm|hmi|xml|zip|bin|dat)\b/i,/saveProject|loadProject|openProject|readProject|writeProject|QDataStream|qCompress|qUncompress|QDomDocument|MainWindow|Download|Compile|Simulation/i];
  const ascii=b.toString('latin1');let scanned=0;
  for(const match of ascii.matchAll(/[\x20-\x7e]{6,256}/g)) {if(++scanned>500000 || Date.now()-started>timeoutMs)throw new Error('String bound exceeded');if(expressions.some(x=>x.test(match[0])))candidates.push({offset:match.index,text:match[0]});if(candidates.length>=2000)break;}
  return {path:relative,sha256:crypto.createHash('sha256').update(b).digest('hex'),machine:'0x'+b.readUInt16LE(pe+4).toString(16),bitness:is64?64:32,clrRva:b.readUInt32LE(dir+14*8),entryRva:b.readUInt32LE(optional+16),sections,imports,exports,stringCandidates:candidates,interpretation:'Static headers and literal candidates only; strings are not behavioral proof.'};
}
const pe = critical.map(inspectPe);
const report={evidenceId:'RESEARCH-INVENTORY',root,date:new Date().toISOString().slice(0,10),bounds:{maxFiles,maxDirectories,maxDepth,timeoutMs},elapsedMs:Date.now()-started,totalFiles:entries.length,totalBytes:entries.reduce((n,x)=>n+x.size,0),directories,skipped,extensions:countBy(x=>x.extension),topDirectories:countBy(x=>x.path.includes('/')?x.path.split('/')[0]:'(root)'),projectCandidates:entries.filter(x=>/\.(hs|knr|laz|sm|hmi|zip)$/i.test(x.path)),files:entries,pe};
fs.mkdirSync(path.dirname(output),{recursive:true});fs.writeFileSync(output,JSON.stringify(report,null,2));
console.log(JSON.stringify({totalFiles:report.totalFiles,totalBytes:report.totalBytes,directories,projectCandidates:report.projectCandidates,primaryImports:pe[0]?.imports.map(x=>x.library)??[],primaryExports:pe[0]?.exports.length??0,output},null,2));
