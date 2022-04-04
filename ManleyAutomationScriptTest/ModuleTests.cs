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
namespace ManleyAutomationScriptTests{
    public class ModuleTests {
        [Fact]
        public void ShouldParseActivity(){
            var module = new Module("My Module","");
            module.Parse("## My Activity");
            Assert.Equal("My Activity",module.Activities.First().Name);
        }
        [Fact]
        public void ShouldParseActivityAfterBlank(){
            var module = new Module("My Module","");
            module.Parse("");
            module.Parse("## My Activity");
            Assert.Equal("My Activity",module.Activities.First().Name);
        }
        [Fact]
        public void ShouldParseMultipleActivities(){
            var module = new Module("My Module","");
            module.Parse("## My Activity");
            module.Parse("## Some Other Activity");
            Assert.Equal("My Activity", module.Activities.First().Name);
            Assert.Equal("Some Other Activity",module.Activities.Last().Name);
        }
        [Fact]
        public void ShouldLetActivitiesParseTheirOwnStuff(){
            var module = new Module("My Module","");
            module.Parse("## My Activity");
            module.Parse("1. This is something else");
            module.Parse("## Some Other Activity");
            Assert.Equal(2,module.Activities.Count);
        }
    }
}