using Nodsoft.WowsReplaysUnpack.Core.Exceptions;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

public interface IDefinitionLoader
{
	/// <summary>
	/// Reads XML data from a .def file (seeking through directories as needed).
	/// </summary>
	/// <param name="clientVersion">Game client version</param>
	/// <param name="name">Name of the .def file to read</param>
	/// <param name="directoryNames">Directories where to search for the .def file</param>
	/// <returns>XML data</returns>
	XmlDocument GetFileAsXml(Version clientVersion, string name, params string[] directoryNames);

	/// <summary>
	/// Gets the root definition directory for a given game client version.
	/// </summary>
	/// <param name="clientVersion">The game client version.</param>
	/// <returns>The root definition directory.</returns>
	DefinitionDirectory GetRootDirectory(Version clientVersion);


	/// <summary>
	/// Finds a definition directory by given names.
	/// </summary>
	/// <param name="clientVersion">The game client version.</param>
	/// <param name="directoryNames">The names of the directories.</param>
	/// <returns>The definition directory.</returns>
	DefinitionDirectory FindDirectory(Version clientVersion, IEnumerable<string> directoryNames);

	/// <summary>
	/// Gets an exact version supported by this definition loader, or the closest version below it.
	/// </summary>
	/// <param name="version">The version to find.</param>
	/// <returns>The closest version supported by this definition loader.</returns>
	/// <exception cref="VersionNotSupportedException">Thrown when no version is supported by this definition loader.</exception>
	public Version GetExactVersion(Version version);
}