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

		private Dictionary<string, string> realToDisplayFileNames = new Dictionary<string, string>();

		public HelpControl()
		{
			Debug.WriteLine("HelpControl created");

			// Could have iterated through all files and read their headers into this dictionary, oh well
			realToDisplayFileNames.Add("what-is-efh2.md", "What is EFH2");
			realToDisplayFileNames.Add("system-requirements.md", "System Requirements");
			realToDisplayFileNames.Add("basic-data-files.md", "Basic Data Files");
			realToDisplayFileNames.Add("curve-number-drainage-area-fields.md", "Curve Number/Drainage Area Fields");
			realToDisplayFileNames.Add("entry-help.md", "Entry Help");
			realToDisplayFileNames.Add("general.md", "General");
			realToDisplayFileNames.Add("rcn-screen.md", "RCN Screen");
			realToDisplayFileNames.Add("state-county.md", "State/County");
			realToDisplayFileNames.Add("time-of-concentration.md", "Time Of Concentration");
			realToDisplayFileNames.Add("edit.md", "Edit");
			realToDisplayFileNames.Add("file.md", "File");
			realToDisplayFileNames.Add("help.md", "Help");
			realToDisplayFileNames.Add("toolbar.md", "Toolbar");
			realToDisplayFileNames.Add("tools.md", "Tools");
			realToDisplayFileNames.Add("view.md", "View");
			realToDisplayFileNames.Add("program-window.md", "Program Window");
			realToDisplayFileNames.Add("rainfall-discharge-data.md", "Rainfall Discharge Data");
			realToDisplayFileNames.Add("glossary.md", "Glossary");

			this.InitializeComponent();

			Task<bool> success = LoadMarkdownAsync(Path.Combine(FileOperations.HelpFileDirectory, @"Introduction\what-is-efh2.md"));

			LoadFilesFromFolder(Path.Combine(FileOperations.HelpFileDirectory));
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
				TreeViewNode node = new TreeViewNode() { Content = realToDisplayFileNames[file.Name] };
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
			Task<bool> success = LoadMarkdownAsync(Path.Combine(FileOperations.HelpFileDirectory, e.Link));
		}

		private void HelpFilesTreeView_SelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs args)
		{

			if (args.AddedItems[0] is TreeViewNode node)
			{
				List<string> keys = realToDisplayFileNames.Where(kvp => kvp.Value == node.Content.ToString()).Select(kvp => kvp.Key).ToList();

				if (keys.Any()) // Found the key
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
