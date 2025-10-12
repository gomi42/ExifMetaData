using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Windows.Controls;

namespace IfcGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonGenTagDetailsRegistry(object sender, RoutedEventArgs e)
        {
            var details = LoadTagDetails();
            GenTagDetailsRegistry(details);
        }

        private void ButtonGenNameExtensionsUser(object sender, RoutedEventArgs e)
        {
            var details = LoadTagDetails();
            GenNameExtensions(details);
        }

        private void ButtonGenTagEnums(object sender, RoutedEventArgs e)
        {
            var details = LoadTagDetails();
            GenTagEnums(details);
        }

        private void ButtonGenTagIdEnums(object sender, RoutedEventArgs e)
        {
            var details = LoadTagDetails();
            GenTagIdEnums(details);
        }

        private void ButtonGenAll(object sender, RoutedEventArgs e)
        {
            GenAll();
        }

        private void GenAll()
        {
            var tagDetails = LoadTagDetails();
            GenTagEnums(tagDetails);
            GenTagIdEnums(tagDetails);
            GenTagDetailsRegistry(tagDetails);
            GenNameExtensions(tagDetails);

            var typeDetails = LoadTypes();
            GenIfdFunctions(typeDetails);
            GenEnumClasses(typeDetails);
        }

        private void ButtonGenIfdFunctions(object sender, RoutedEventArgs e)
        {
            GenIfdFunctions(LoadTypes());
        }

        private void ButtonGenEnumClasses(object sender, RoutedEventArgs e)
        {
            GenEnumClasses(LoadTypes());
        }

        private void GenTagDetailsRegistry(List<TagDetails> details)
        {
            var allTypes = LoadTypes();
            StreamWriter writer = File.CreateText(Path.Combine(GetExifPath(), @"Data\TagDetailsRegistry.cs"));

            WriteAutoHeader(writer);
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("");
            writer.WriteLine("namespace ExifMeta");
            writer.WriteLine("{");
            writer.WriteLine("    internal static partial class TagDetailsRegistry");
            writer.WriteLine("    {");
            writer.WriteLine("        private static Dictionary<TagId, TagDetails> tagDetails = new Dictionary<TagId, TagDetails>()");
            writer.WriteLine("        {");

            var indent1 = "            ";
            var indent2 = "                ";
            var indent3 = "                               ";

            foreach (var tag in details)
            {
                writer.WriteLine($"{indent1}{{");
                writer.WriteLine($"{indent2}TagId.{tag.Name},");
                writer.WriteLine($"{indent2}new TagDetails(IfdId.{tag.Ifd},");
                writer.WriteLine($"{indent3}Tag.{tag.Name},");

                // datatype
                var finalTypes = tag.Types.Select(x => "DataType." + x);

                if (tag.Types.Count() == 1)
                {
                    writer.WriteLine($"{indent3}{finalTypes.First()},");
                }
                else
                {
                    writer.WriteLine($"{indent3}new[] {{ {string.Join(", ", finalTypes)} }},");
                }

                // user
                var isSystem = tag.IsUser ? "true" : "false";
                writer.WriteLine($"{indent3}{isSystem},");

                // read-only
                var isReadOnly = tag.ReadOnly ? "true" : "false";
                writer.WriteLine($"{indent3}{isReadOnly},");

                // class name
                string propertyClassName = null;

                if (string.IsNullOrEmpty(tag.Class))
                {
                    var t = allTypes.Find(x => x.Class == tag.Types[0]);
                    propertyClassName = t.Class;

                    if (!string.IsNullOrEmpty(t.Function))
                    {
                        propertyClassName = t.Function;
                    }
                    else
                    {
                        propertyClassName = t.Class;
                    }
                }
                else
                {
                    propertyClassName = $"{tag.Class}";
                }

                if (!string.IsNullOrEmpty(tag.Class) && string.IsNullOrEmpty(propertyClassName))
                {
                    throw new Exception("missing or wrong type definition");
                }

                if (!string.IsNullOrEmpty(propertyClassName))
                {
                    writer.Write($"{indent3}typeof({propertyClassName}Property)");
                }
                else
                {
                    writer.Write($"{indent3}null");
                }

                // count
                if (!string.IsNullOrEmpty(tag.Count))
                {
                    writer.WriteLine(",");
                    writer.Write($"{indent3}{tag.Count}");
                }
                else
                {
                    if (tag.Types[0] == "Ascii"
                        || tag.Types[0] == "Byte"
                        || tag.Types[0] == "Undefined")
                    {
                        writer.WriteLine(",");
                        writer.Write($"{indent3}-1");
                    }
                }

                writer.WriteLine(")");

                // collect object initializier
                var inits = new List<string>();

                // payload
                if (tag.DontLoadPayload)
                {
                    var payload = tag.DontLoadPayload ? "true" : "false";
                    inits.Add($"DontLoadPayload = {payload}");
                }

                // display converter
                if (!string.IsNullOrEmpty(tag.DisplayConverter))
                {
                    inits.Add($"DisplayConverter = new {tag.DisplayConverter}()");
                }

                // write object initializier
                if (inits.Count == 1)
                {
                    writer.WriteLine($"{indent3}{{ {inits[0]} }}");
                }
                else
                if (inits.Count > 1)
                {
                    writer.WriteLine($"{indent3}{{");

                    var args = string.Join($",\r\n{indent3}    ", inits);
                    writer.WriteLine($"{indent3}    {args}");
                    writer.WriteLine($"{indent3}}}");
                }

                writer.WriteLine($"{indent1}}},");
            }

            writer.WriteLine("        };");
            writer.WriteLine("    }");
            writer.WriteLine("}");

            writer.Close();
        }

        private void GenNameExtensions(List<TagDetails> details)
        {
            var allTypes = LoadTypes();
            StreamWriter writer = File.CreateText(Path.Combine(GetExifPath(), @"Extensions\UserExtensions.cs"));

            WriteAutoHeader(writer);
            writer.WriteLine("using System;");
            writer.WriteLine("");
            writer.WriteLine("namespace ExifMeta");
            writer.WriteLine("{");
            writer.WriteLine("    public static class UserExtensions");
            writer.WriteLine("    {");

            var separator = new WriteSeparator(writer);

            foreach (var tag in details)
            {
                if (tag.NoAccessFunction)
                {
                    continue;
                }

                separator.Next(tag.Ifd);
                var valueCount = 1;

                if (!string.IsNullOrEmpty(tag.Count))
                {
                    if (int.TryParse(tag.Count, out int result))
                    {
                        valueCount = result;
                    }
                }

                var tagName = tag.Name;
                var tagDataType = tag.Types[0];
                var propertyClassName = tag.Class;
                bool isReadOnly = tag.ReadOnly;
                string methodName = string.Empty;
                string methodType = string.Empty;
                string classtoFind;

                if (!string.IsNullOrEmpty(propertyClassName))
                {
                    classtoFind = propertyClassName;
                }
                else
                {
                    classtoFind = tagDataType;
                }

                var t = allTypes.Find(x => x.Class == classtoFind);
                methodType = t.Type;

                if (!string.IsNullOrEmpty(t.Function))
                {
                    methodName = t.Function;
                }
                else
                {
                    methodName = t.Class;
                }

                if (string.IsNullOrEmpty(methodName))
                {
                    throw new Exception("missing or wrong type definition");
                }

                if (methodName != "String" && valueCount != 1)
                {
                    methodName += "s";
                    methodType += "[]";
                }

                writer.WriteLine($"        public static {methodType} Get{tagName}(this Ifd ifd) => ifd.Get{methodName}Property(TagId.{tagName});");

                if (!isReadOnly)
                {
                    writer.WriteLine($"        public static void Set{tagName}(this Ifd ifd, {methodType} value) => ifd.Set{methodName}Property(TagId.{tagName}, value);");
                }
            }

            writer.WriteLine("    }");
            writer.WriteLine("}");

            writer.Close();
        }

        private void GenTagEnums(List<TagDetails> details)
        {
            StreamWriter writer = File.CreateText(Path.Combine(GetExifPath(), @"Data\Tag.cs"));

            WriteAutoHeader(writer);
            writer.WriteLine("namespace ExifMeta");
            writer.WriteLine("{");
            writer.WriteLine("    internal enum Tag");
            writer.WriteLine("    {");

            var listAllTagIds = new List<string>();
            var separator = new WriteSeparator(writer);

            foreach (var tag in details)
            {
                listAllTagIds.Add(tag.Tag);
                separator.Next(tag.Ifd);
                writer.WriteLine($"        {tag.Name} = {tag.Tag},");
            }

            writer.WriteLine("    }");
            writer.WriteLine("}");

            writer.Close();

            var duplicates = listAllTagIds
                                    .GroupBy(x => x)
                                    .Where(g => g.Count() > 1)
                                    .Select(g => g.Key)
                                    .ToList();

            if (duplicates.Count > 1)
            {
                throw new Exception("there are duplicate entries");
            }
        }

        private void GenTagIdEnums(List<TagDetails> details)
        {
            StreamWriter writer = File.CreateText(Path.Combine(GetExifPath(), @"Data\TagId.cs"));

            WriteAutoHeader(writer);
            writer.WriteLine("namespace ExifMeta");
            writer.WriteLine("{");
            writer.WriteLine("    public enum TagId");
            writer.WriteLine("    {");

            var separator = new WriteSeparator(writer);

            foreach (var tag in details)
            {
                separator.Next(tag.Ifd);
                writer.WriteLine($"        {tag.Name},");
            }

            writer.WriteLine("    }");
            writer.WriteLine("}");

            writer.Close();
        }

        private string GetExifPath()
        {
            string exePath = Assembly.GetExecutingAssembly().Location;
            string exeDirectory = Path.GetDirectoryName(exePath);
            return Path.Combine(exeDirectory, @"..\..\..\ExifMeta");
        }

        private void WriteAutoHeader(StreamWriter writer)
        {
            writer.WriteLine("// Automatically generated file. Do not modify.");
            writer.WriteLine("");
        }

        private List<TagDetails> LoadTagDetails()
        {
            var reader = File.OpenRead(Path.Combine(GetExifPath(), @"AllTags.json"));
            var serializer = new DataContractJsonSerializer(typeof(List<TagDetails>));
            var details = (List<TagDetails>)serializer.ReadObject(reader);
            reader.Close();

            details.Sort(delegate (TagDetails x, TagDetails y)
            {
                var prios = new Dictionary<string, int>()
                {
                    { "Standard", 1 },
                    { "Exif", 2 },
                    { "Gps", 3 }
                };

                var xIfd = prios[x.Ifd];
                var yIfd = prios[y.Ifd];

                var c = xIfd.CompareTo(yIfd);

                if (c != 0)
                {
                    return c;
                }

                return x.Tag.CompareTo(y.Tag);
            });

            return details;
        }

        [DataContract]
        private struct TagDetails
        {
            [DataMember]
            public string Name { get; set; }

            [DataMember]
            public string Ifd { get; set; }

            [DataMember]
            public string Tag { get; set; }

            [DataMember]
            public string[] Types { get; set; }

            [DataMember]
            public string Count { get; set; }

            [DataMember]
            public string Class { get; set; }

            [DataMember(Name = "User")]
            public bool IsUser { get; set; }

            [DataMember]
            public bool ReadOnly { get; set; }

            [DataMember]
            public bool NoAccessFunction { get; set; }

            [DataMember]
            public bool DontLoadPayload { get; set; }

            [DataMember]
            public string DisplayConverter { get; set; }
        }

        private void GenIfdFunctions(List<TypeDetails> typeDetails)
        {
            StreamWriter writer = File.CreateText(Path.Combine(GetExifPath(), @"Core\IfdProperties.cs"));

            WriteAutoHeader(writer);
            writer.WriteLine("using System;");
            writer.WriteLine("");
            writer.WriteLine("namespace ExifMeta");
            writer.WriteLine("{");
            writer.WriteLine("    public partial class Ifd");
            writer.WriteLine("    {");

            var indent1 = "        ";
            var indent2 = "            ";

            foreach (var detail in typeDetails)
            {
                var propertyClassName = detail.Class;
                var type = detail.Type;
                var single = detail.Single;
                var plural = detail.Plural;

                if (detail.NoIfdFunction)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(detail.Function))
                {
                    propertyClassName = detail.Function;
                }

                writer.WriteLine($"{indent1}public {type} Get{propertyClassName}Property(TagId tagId)");
                writer.WriteLine($"{indent1}{{");

                if (single)
                {
                    writer.WriteLine($"{indent2}return (({propertyClassName}Property)GetProperty(tagId)).Value;");
                }
                else
                {
                    writer.WriteLine($"{indent2}return (({propertyClassName}Property)GetProperty(tagId)).Values[0];");
                }

                writer.WriteLine($"{indent1}}}");
                writer.WriteLine();

                if (plural)
                {
                    writer.WriteLine($"{indent1}public {type}[] Get{propertyClassName}sProperty(TagId tagId)");
                    writer.WriteLine($"{indent1}{{");
                    writer.WriteLine($"{indent2}return (({propertyClassName}Property)GetProperty(tagId)).Values;");
                    writer.WriteLine($"{indent1}}}");
                    writer.WriteLine();
                }

                writer.WriteLine($"{indent1}public void Set{propertyClassName}Property(TagId tagId, {type} value)");
                writer.WriteLine($"{indent1}{{");
                writer.WriteLine($"{indent2}SetProperty(new {propertyClassName}Property(tagId, value));");
                writer.WriteLine($"{indent1}}}");
                writer.WriteLine();

                if (plural)
                {
                    writer.WriteLine($"{indent1}public void Set{propertyClassName}sProperty(TagId tagId, {type}[] values)");
                    writer.WriteLine($"{indent1}{{");
                    writer.WriteLine($"{indent2}SetProperty(new {propertyClassName}Property(tagId, values));");
                    writer.WriteLine($"{indent1}}}");
                    writer.WriteLine();
                }
            }

            writer.WriteLine("    }");
            writer.WriteLine("}");

            writer.Close();
        }

        private void GenEnumClasses(List<TypeDetails> typeDetails)
        {
            StreamWriter writer = File.CreateText(Path.Combine(GetExifPath(), @"Property\EnumProperties.cs"));

            WriteAutoHeader(writer);
            writer.WriteLine("using System.Diagnostics;");
            writer.WriteLine("");
            writer.WriteLine("namespace ExifMeta");
            writer.WriteLine("{");

            var indent1 = "        ";
            var indent2 = "            ";

            foreach (var detail in typeDetails)
            {
                var type = detail.Type;
                var single = detail.Single;
                var plural = detail.Plural;
                var baseType = detail.EnumBase;

                if (!detail.Enum || detail.NoIfdFunction)
                {
                    continue;
                }

                writer.WriteLine("    [DebuggerDisplay(\"{TagId} = {Value}\")]");
                writer.WriteLine($"    public class {type}Property : EnumProperty<{type}>");
                writer.WriteLine("    {");

                writer.WriteLine($"{indent1}public {type}Property(TagId tagId, {type} value) : base(tagId, value, DataType.{baseType}) {{}}");
                writer.WriteLine("    }");
                writer.WriteLine();
            }

            writer.WriteLine("}");

            writer.Close();
        }

        private List<TypeDetails> LoadTypes()
        {
            var reader = File.OpenRead(Path.Combine(GetExifPath(), @"AllTypes.json"));
            var serializer = new DataContractJsonSerializer(typeof(List<TypeDetails>));
            var details = (List<TypeDetails>)serializer.ReadObject(reader);
            reader.Close();

            //var code = File.ReadAllText("Pfad/zur/Datei.cs");
            //var tree = CSharpSyntaxTree.ParseText(code);
            //var root = tree.GetRoot();

            //var enums = root.DescendantNodes()
            //            .OfType<EnumDeclarationSyntax>();

            //foreach (var enumDecl in enums)
            //{
            //    var name = enumDecl.Identifier.Text;
            //    var baseType = enumDecl.BaseList?.Types.FirstOrDefault();

            //    details.Add(new TypeDetails()
            //                            {
            //                                Function = enumName,
            //                                Type = enumName,
            //                                Plural = false,
            //                                Single = tree,
            //                                Enum = true,
            //                                Base = ...
            //                            });
            //}

            return details;
        }

        [DataContract]
        private struct TypeDetails
        {
            [DataMember]
            public string Class { get; set; }

            [DataMember]
            public string Function { get; set; }

            [DataMember]
            public string Type { get; set; }

            [DataMember]
            public bool Single { get; set; }

            [DataMember]
            public bool Plural { get; set; }

            [DataMember]
            public bool Enum { get; set; }

            [DataMember]
            public string EnumBase { get; set; }

            [DataMember]
            public bool NoIfdFunction { get; set; }
        }

        private class WriteSeparator
        {
            private string lastName = string.Empty;
            private StreamWriter writer;

            public WriteSeparator(StreamWriter writer)
            {
                this.writer = writer;
            }

            public void Next(string name)
            {
                if (name != lastName)
                {
                    if (!string.IsNullOrEmpty(lastName))
                    {
                        writer.WriteLine("");
                    }

                    lastName = name;
                    writer.WriteLine($"        // {lastName}");
                }
            }
        }
    }
}
