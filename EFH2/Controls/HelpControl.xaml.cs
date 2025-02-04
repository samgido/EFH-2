using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH2
{
	public sealed partial class HelpControl : UserControl
	{
		// These files in the initial markdown file need to be replaced with their full path
		private List<string> replacedFiles = new List<string>() { };
		private List<string> unqualifiedImageNames = new List<string>();

		private Dictionary<string, string> realToDisplayFileNames = new Dictionary<string, string>();

		public HelpControl()
		{
			// Qualify image paths
			foreach (FileInfo f in new DirectoryInfo(FileOperations.HelpFileDirectory).GetFiles())
			{
				if (f.Extension == ".png")
				{
					unqualifiedImageNames.Add(f.Name);
				}
			}

			QualifyImagePaths(FileOperations.HelpFileDirectory);

			Debug.WriteLine("HelpControl created");

			// Could have iterated through all files and read their headers into this dictionary, oh well
			// Introduction folder
			realToDisplayFileNames.Add(Path.Combine(FileOperations.HelpFileDirectory, "Introduction", "what-is-efh2.md"), "What is EFH2");
			realToDisplayFileNames.Add(Path.Combine(FileOperations.HelpFileDirectory, "Introduction", "system-requirements.md"), "System Requirements");
			realToDisplayFileNames.Add(Path.Combine(FileOperations.HelpFileDirectory, "Introduction", "basic-data-files.md"), "Basic Data Files");

			// Data Entry folder
			realToDisplayFileNames.Add(Path.Combine(FileOperations.HelpFileDirectory, @"Data Entry", "curve-number-drainage-area-fields.md"), "Curve Number/Drainage Area Fields");
			realToDisplayFileNames.Add(Path.Combine(FileOperations.HelpFileDirectory, @"Data Entry", "entry-help.md"), "Entry Help");
			realToDisplayFileNames.Add(Path.Combine(FileOperations.HelpFileDirectory, @"Data Entry", "general.md"), "General");
			realToDisplayFileNames.Add(Path.Combine(FileOperations.HelpFileDirectory, @"Data Entry", "rcn-screen.md"), "RCN Screen");
			realToDisplayFileNames.Add(Path.Combine(FileOperations.HelpFileDirectory, @"Data Entry", "state-county.md"), "State/County");
			realToDisplayFileNames.Add(Path.Combine(FileOperations.HelpFileDirectory, @"Data Entry", "time-of-concentration.md"), "Time Of Concentration");

			// User Interface\Menu folder
			realToDisplayFileNames.Add(Path.Combine(FileOperations.HelpFileDirectory, @"User Interface", "Menu", "edit.md"), "Edit");
			realToDisplayFileNames.Add(Path.Combine(FileOperations.HelpFileDirectory, @"User Interface", "Menu", "file.md"), "File");
			realToDisplayFileNames.Add(Path.Combine(FileOperations.HelpFileDirectory, @"User Interface", "Menu", "help.md"), "Help");
			realToDisplayFileNames.Add(Path.Combine(FileOperations.HelpFileDirectory, @"User Interface", "Menu", "toolbar.md"), "Toolbar");
			realToDisplayFileNames.Add(Path.Combine(FileOperations.HelpFileDirectory, @"User Interface", "Menu", "tools.md"), "Tools");
			realToDisplayFileNames.Add(Path.Combine(FileOperations.HelpFileDirectory, @"User Interface", "Menu", "view.md"), "View");

			// User Interface folder
			realToDisplayFileNames.Add(Path.Combine(FileOperations.HelpFileDirectory, @"User Interface", "program-window.md"), "Program Window");
			realToDisplayFileNames.Add(Path.Combine(FileOperations.HelpFileDirectory, @"User Interface", "rainfall-discharge-data.md"), "Rainfall Discharge Data");

			// Root help directory
			realToDisplayFileNames.Add(Path.Combine(FileOperations.HelpFileDirectory, "glossary.md"), "Glossary");

			this.InitializeComponent();

			Task<bool> success = LoadMarkdownAsync(Path.Combine(FileOperations.HelpFileDirectory, @"Introduction\what-is-efh2.md"));

			LoadFilesFromFolder(FileOperations.HelpFileDirectory);

			ExpandAll(HelpFilesTreeView.RootNodes[0]);
		}

		private void QualifyImagePaths(string path)
		{
			DirectoryInfo dirInfo = new DirectoryInfo(path);

			foreach (FileInfo file in dirInfo.GetFiles())
			{
				if (file.Extension != ".md") { continue; }

				using (StreamReader sr = new StreamReader(file.FullName))
				{
					string content = sr.ReadToEnd();
					foreach (string image in unqualifiedImageNames)
					{
						if (content.Contains(image))
						{
							content = content.Replace(image, Path.Combine(FileOperations.HelpFileDirectory, image));
						}
					}

					sr.Close();
					File.WriteAllText(file.FullName, content);
				}
			}

			foreach (DirectoryInfo dir in dirInfo.GetDirectories())
			{
				QualifyImagePaths(dir.FullName);
			}
		}

		private void ExpandAll(TreeViewNode node)
		{
			node.IsExpanded = true;

			foreach (TreeViewNode child in node.Children)
			{
				ExpandAll(child);
			}
		}

		private void LoadFilesFromFolder(string path)
		{
			DirectoryInfo dirInfo = new DirectoryInfo(path);

			HelpFilesTreeView.RootNodes.Clear();

			TreeViewNode rootNode = new TreeViewNode() { Content = dirInfo.Name };

			AddDirectoryNodes(rootNode, dirInfo);

			HelpFilesTreeView.RootNodes.Add(rootNode);
		}

		private void AddDirectoryNodes(TreeViewNode rootNode, DirectoryInfo dirInfo)
		{
			foreach (DirectoryInfo dir in dirInfo.GetDirectories())
			{
				TreeViewNode node = new TreeViewNode() { Content = dir.Name };
				rootNode.Children.Add(node);
				AddDirectoryNodes(node, dir);
			}

			foreach (FileInfo file in dirInfo.GetFiles("*.md"))
			{
				TreeViewNode node = new TreeViewNode() { Content = realToDisplayFileNames[file.FullName] };
				rootNode.Children.Add(node);
			}
		}

		private async Task<bool> LoadMarkdownAsync(string path)
		{
			try
			{
				if (!File.Exists(path))
				{
					Debug.WriteLine("Can't find " + path);
					return false;
				}
				else
				{
					Debug.WriteLine("Navigating to " + path);
				}

				string content = await File.ReadAllTextAsync(path);

				foreach (string image in replacedFiles)
				{
					content = content.Replace(image, Path.Combine(FileOperations.HelpFileDirectory, image));
				}

				MainTextBlock.Text = content;

				return true;
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
				return false;
			}
		}

		private void MainTextBlock_LinkClicked(object sender, CommunityToolkit.WinUI.UI.Controls.LinkClickedEventArgs e)
		{
			Task<bool> success = LoadMarkdownAsync(Path.Combine(FileOperations.HelpFileDirectory, e.Link.Replace("%20", " ")));
		}

		private void HelpFilesTreeView_SelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs args)
		{
			if (args.AddedItems[0] is TreeViewNode node)
			{
				List<string> keys = realToDisplayFileNames.Where(kvp => kvp.Value == node.Content.ToString()).Select(kvp => kvp.Key).ToList();

				if (keys.Any()) // Found a key
				{
					Task<bool> success = LoadMarkdownAsync(Path.Combine(FileOperations.HelpFileDirectory, keys[0]));
				}
			}
		}
	}

	public class FileItem
	{
		public string Name { get; set; }
		public string Path { get; set; }
		public FileItem(string name, string path)
		{
			Name = name;
			Path = path;
		}
	}
}
