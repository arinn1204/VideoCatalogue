using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoreLinq;
using System.Reflection;
using System.Text;
using Xunit.Sdk;

namespace Grains.Tests.Unit.Attributes
{
#nullable enable
    public class CreateMkvTestDataAttribute : BeforeAfterTestAttribute
    {
        private readonly int _numberOfFilesToCreate;
        private IEnumerable<(string, string?)> _createdPaths;

        public CreateMkvTestDataAttribute(int numberOfFilesToCreate)
        {
            _numberOfFilesToCreate = numberOfFilesToCreate;
            _createdPaths = Enumerable.Empty<(string, string?)>();
        }
        public override void After(MethodInfo methodUnderTest)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            base.After(methodUnderTest);

            foreach(var path in _createdPaths)
            {
                try
                {
                    File.Delete(path.Item1);
                    Directory.Delete(path.Item2, true);
                }
                catch (Exception e)
                {

                }
                
            }
        }

        public override void Before(MethodInfo methodUnderTest)
        {
            base.Before(methodUnderTest);

            var testDataFile = Path.Combine("TestData", "VideoSearcher", "filesWithoutFeaturettes.txt");

            var filesToCreate = File.ReadLines(testDataFile)
                                    .RandomSubset(_numberOfFilesToCreate);

            var itemsToCreate = filesToCreate.Select(s =>
                {
                    var fileStructure = s.Split(new[] { '/', '\\' });

                    fileStructure = fileStructure[0] == "."
                        ? fileStructure.Skip(1).ToArray()
                        : fileStructure.ToArray();

                    var filePath = Path.Combine(fileStructure);
                    var new_file = Path.Combine("TestData", filePath);

                    return new_file;
                })
                .Distinct();

            foreach(var file in itemsToCreate)
            {
                if (!Directory.Exists(Path.GetDirectoryName(file))) 
                {
                    var directoryName = Path.GetDirectoryName(file);
                    var info = Directory.CreateDirectory(directoryName);
                    _createdPaths = Enumerable.Append(
                        _createdPaths,
                        (file, directoryName));
                }

                File.Create(file);
            }
        }
    }
}
