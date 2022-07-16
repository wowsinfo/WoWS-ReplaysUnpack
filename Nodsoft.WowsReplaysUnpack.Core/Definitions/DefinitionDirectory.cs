using Nodsoft.WowsReplaysUnpack.Core.Extensions;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

public class DefinitionDirectory
{
	public string Name { get; }
	public string Path { get; }

	public List<DefinitionFile> Files { get; } = new();
	public List<DefinitionDirectory> Directories { get; } = new();

	public DefinitionDirectory(string name, string path, string[] fileNames)
	{
		Name = name;
		Path = path;
		ParseChildrenRecursive(fileNames);
	}

	private void ParseChildrenRecursive(string[] fileNames)
	{
		fileNames = fileNames.Where(name => name.StartsWith(Path)).ToArray();

		foreach (string fileName in fileNames)
		{
			string actualFileName = fileName.GetStringAfterLength(Path + ".");

			if (actualFileName.Count(c => c == '.') is 1)
			{
				Files.Add(new(actualFileName, fileName));
			}
			else
			{
				string directoryName = actualFileName.GetStringBeforeIndex('.');

				if (Directories.All(d => d.Name != directoryName))
				{
					Directories.Add(new(actualFileName.GetStringBeforeIndex('.'), fileName.GetStringBeforeIndex("." + actualFileName.GetStringAfterIndex('.')), fileNames));
				}
			}
		}
	}
}

public record DefinitionFile(string Name, string Path);