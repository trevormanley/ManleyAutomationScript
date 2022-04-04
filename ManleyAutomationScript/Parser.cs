/*
Copyright 2022 Trevor Manley

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
namespace ManleyAutomationScript{
    public class Parser{
        public List<Module> Modules = new();
        public void Parse(string script){
            var byLines = script.Split("\n");
            var copyrightStatement = "";
            var isPreamble = false;
            foreach(var line in byLines){
                if(line.StartsWith("# ")){
                    var moduleName = line.Replace("#","").Trim();
                    Modules.Add(new Module(moduleName, copyrightStatement));
                    continue;
                }
                if(Modules.Any()){
                    Modules.Last().Parse(line);
                    continue;
                }
                if(line.StartsWith("---") && isPreamble){
                    isPreamble = false;
                    continue;
                }
                if(isPreamble){
                    copyrightStatement += line + "\n";
                }
                if(line.StartsWith("---") && !isPreamble){
                    isPreamble = true;
                }
            }
        }
    }
}