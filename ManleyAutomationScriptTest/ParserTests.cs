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
using Xunit;
using ManleyAutomationScript;
using System.Linq;
namespace ManleyAutomationScriptTests {
    public class ParserTests{
        [Fact]
        public void ShouldMakeNewModuleWithCopyright(){
            var parser = new Parser();
var script = @"---
This is some statement
and again
and again
---
# My Module
";
            parser.Parse(script);
            var mod = parser.Modules.First();
            Assert.Equal("This is some statement\nand again\nand again\n", mod.CopyrightStatement);
        }
        [Fact]
        public void ShouldMakeNewModuleWithName(){
            var parser = new Parser();
var script = @"---
This is some statement
and again
and again
---
# My Module              
--- There is a bunch of whitespace above
";
            parser.Parse(script);
            var mod = parser.Modules.First();
            Assert.Equal("My Module", mod.Name);
        }
        [Fact]
        public void ShouldMakeNewModuleWithNameAndSameCopyright(){
            var parser = new Parser();
var script = @"---
This is some statement
---
# My Module
---
Stuff for My Module
---
# Another One
";
            parser.Parse(script);
            var module1 = parser.Modules.First();
            var module2 = parser.Modules.Last();
            Assert.Equal("My Module", module1.Name);
            Assert.Equal("Another One", module2.Name);
            Assert.Equal("This is some statement\n", module2.CopyrightStatement);
        }
    }
}