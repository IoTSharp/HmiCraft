import fs from 'node:fs';
import path from 'node:path';
import {spawn} from 'node:child_process';
import {once} from 'node:events';
import readline from 'node:readline';

const mode=process.argv[2], output=process.argv[3];
if(!mode||!output||!path.isAbsolute(output))throw new Error('mode and absolute evidence directory required');
fs.mkdirSync(output,{recursive:true});
const child=spawn('C:/Program Files/nodejs/node.exe',['C:/Users/mysti/AppData/Roaming/npm/node_modules/rea-agents/scripts/rea.mjs','mcp'],{
  windowsHide:true,stdio:['pipe','pipe','pipe'],env:{...process.env,GHIDRA_INSTALL_DIR:'C:/Users/mysti/.local/share/ghidra/ghidra_12.1.4_PUBLIC',JAVA_HOME:'C:/Program Files/Microsoft/jdk-21.0.7.6-hotspot',GHIDRA_HEADLESS_MAXMEM:process.env.GHIDRA_HEADLESS_MAXMEM??'1G',REA_GHIDRA_STARTUP_TIMEOUT_MS:process.env.REA_GHIDRA_STARTUP_TIMEOUT_MS??'300000'}
});
const exited=once(child,'exit'), pending=new Map();let sequence=0,linesCount=0,opened=false;
const stderr=fs.createWriteStream(path.join(output,`rea-${path.basename(mode,'.json')}-stderr.log`),{flags:'wx'});
child.stderr.pipe(stderr);
const lines=readline.createInterface({input:child.stdout});
const deadline=setTimeout(()=>{for(const task of pending.values())task.reject(new Error('REA session overall deadline'));child.stdin.end();},540000);
lines.on('line',line=>{if(++linesCount>10000){child.stdin.end();return;}try{const response=JSON.parse(line);const task=pending.get(response.id);if(task){pending.delete(response.id);clearTimeout(task.timer);response.error?task.reject(new Error(JSON.stringify(response.error))):task.resolve(response.result);}}catch(error){for(const task of pending.values())task.reject(error);}});
function request(method,params,timeout=30000){const id=++sequence;return new Promise((resolve,reject)=>{const timer=setTimeout(()=>{pending.delete(id);reject(new Error(`${method} request deadline`));},timeout);pending.set(id,{resolve,reject,timer});child.stdin.write(JSON.stringify({jsonrpc:'2.0',id,method,params})+'\n');});}
async function call(name,args,file,timeout=60000){console.log(`REA ${name}`);const result=await request('tools/call',{name,arguments:args},timeout);fs.writeFileSync(path.join(output,file),JSON.stringify(result,null,2),{flag:'wx'});if(result.isError)throw new Error(`REA ${name} returned error; see ${file}`);return result;}
try{
 const initialized=await request('initialize',{protocolVersion:'2025-03-26',capabilities:{},clientInfo:{name:'HmiStudio evidence collection',version:'0.1.0'}});
 child.stdin.write(JSON.stringify({jsonrpc:'2.0',method:'notifications/initialized'})+'\n');
 const catalog=await request('tools/list',{});
 if(mode==='catalog'){
  const tools=catalog.tools.filter(tool=>/open_binary|binary_overview|binary_session|search|xref|decomp|analyze_function|close_binary/.test(tool.name));
  fs.writeFileSync(path.join(output,'rea-tool-schemas.json'),JSON.stringify(tools,null,2));
  console.log(JSON.stringify(tools.map(tool=>({name:tool.name,inputSchema:tool.inputSchema})),null,2));
 }else{
  const plan=JSON.parse(fs.readFileSync(mode,'utf8'));
  if(plan.length>12)throw new Error('At most 12 planned operations per session');
  for(const [index,step]of plan.entries()){
   if(step.name==='open_binary')opened=true;
   await call(step.name,step.arguments,step.file??`rea-step-${index}.json`,step.timeout??60000);
   if(step.name==='close_binary')opened=false;
  }
 }
 console.log(JSON.stringify({server:initialized.serverInfo,completed:true,serverPid:child.pid}));
}finally{
 if(opened)try{await call('close_binary',{},`rea-${path.basename(mode,'.json')}-close-finally.json`,45000);}catch(error){console.error(String(error));}
 child.stdin.end();const timer=setTimeout(()=>child.kill(),8000);
 await exited;clearTimeout(timer);clearTimeout(deadline);lines.close();stderr.end();
 for(const task of pending.values())clearTimeout(task.timer);
}
