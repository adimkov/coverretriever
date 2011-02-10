using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace CoverRetriever.Helpers
{
	public static class TreeViewExtensions
	{

		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.RegisterAttached("SelectedItem", typeof(IEnumerable<object>), typeof(TreeViewExtensions),
				new PropertyMetadata(new PropertyChangedCallback(OnSelectedItemChanged)));


		public static void SetSelectedItem(DependencyObject o, object value)
		{
			o.SetValue(SelectedItemProperty, value);
		}

		public static IEnumerable<object> GetSelectedItem(DependencyObject o)
		{
			return (IEnumerable<object>)o.GetValue(SelectedItemProperty);
		}
		
		private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var treeView = d as TreeView;
			if (treeView != null && e.NewValue != null)
			{
				treeView.SelectItem((IEnumerable<object>)e.NewValue);
			}
		}

		/// <summary>
		/// Selects an item in a TreeView using a path
		/// </summary>
		/// <param name="treeView">The TreeView to select an item in</param>
		/// <param name="path">The path to the selected item.
		/// Components of the path are separated with Path.DirectorySeparatorChar.
		/// Items in the control are converted by calling the ToString method.</param>
		public static void SelectItem(this TreeView treeView, string path)
		{
			treeView.SelectItem(path, item => item.ToString());
		}

		/// <summary>
		/// Selects an item in a TreeView using a path and a custom conversion method
		/// </summary>
		/// <param name="treeView">The TreeView to select an item in</param>
		/// <param name="path">The path to the selected item.
		/// Components of the path are separated with Path.DirectorySeparatorChar.</param>
		/// <param name="convertMethod">A custom method that converts items in the control to their respective path component</param>
		public static void SelectItem(this TreeView treeView, string path,
			Func<object, string> convertMethod)
		{
			treeView.SelectItem(path, convertMethod, Path.DirectorySeparatorChar);
		}

		/// <summary>
		/// Selects an item in a TreeView using a path and a custom path separator character.
		/// </summary>
		/// <param name="treeView">The TreeView to select an item in</param>
		/// <param name="path">The path to the selected item</param>
		/// <param name="separatorChar">The character that separates path components</param>
		public static void SelectItem(this TreeView treeView, string path,
			char separatorChar)
		{
			treeView.SelectItem(path, item => item.ToString(), separatorChar);
		}

		/// <summary>
		/// Selects an item in a TreeView using a path, a custom conversion method,
		/// and a custom path separator character.
		/// </summary>
		/// <param name="treeView">The TreeView to select an item in</param>
		/// <param name="path">The path to the selected item</param>
		/// <param name="convertMethod">A custom method that converts items in the control to their respective path component</param>
		/// <param name="separatorChar">The character that separates path components</param>
		public static void SelectItem(this TreeView treeView, string path,
			Func<object, string> convertMethod, char separatorChar)
		{
			treeView.SelectItem<string>(
				path.Split(new char[] { separatorChar },
					StringSplitOptions.RemoveEmptyEntries),
				(x, y) => x == y,
				convertMethod
			);
		}

		/// <summary>
		/// Selects an item in a TreeView using a custom item chain
		/// </summary>
		/// <typeparam name="T">The type of the items present in the control and the chain</typeparam>
		/// <param name="treeView">The TreeView to select an item in</param>
		/// <param name="items">The chain of items to walk. The last item in the chain will be selected</param>
		public static void SelectItem<T>(this TreeView treeView, IEnumerable<T> items)
			where T : class
		{
			// Use a default compare method with the '==' operator
			treeView.SelectItem<T>(items,
				(x, y) => x == y
			);
		}

		/// <summary>
		/// Selects an item in a TreeView using a custom item chain and item comparison method
		/// </summary>
		/// <typeparam name="T">The type of the items present in the control and the chain</typeparam>
		/// <param name="treeView">The TreeView to select an item in</param>
		/// <param name="items">The chain of items to walk. The last item in the chain will be selected</param>
		/// <param name="compareMethod">The method used to compare items in the control with items in the chain</param>
		public static void SelectItem<T>(this TreeView treeView, IEnumerable<T> items,
			Func<T, T, bool> compareMethod)
		{
			treeView.SelectItem<T>(items, compareMethod, null);
		}

		/// <summary>
		/// Selects an item in a TreeView using a custom item chain, an item comparison method,
		/// and an item conversion method.
		/// </summary>
		/// <typeparam name="T">The type of the items present in the control and the chain</typeparam>
		/// <param name="treeView">The TreeView to select an item in</param>
		/// <param name="items">The chain of items to walk. The last item in the chain will be selected</param>
		/// <param name="compareMethod">The method used to compare items in the control with items in the chain</param>
		/// <param name="convertMethod">The method used to convert items in the control to be compared with items in the chain</param>
		public static void SelectItem<T>(this TreeView treeView, IEnumerable<T> items,
			Func<T, T, bool> compareMethod, Func<object, T> convertMethod)
		{
			// Setup default options for a TreeView
			UIUtility.SetSelectedItem<T>(treeView,
				new SetSelectedInfo<T>()
				{
					Items = items,
					CompareMethod = compareMethod,
					ConvertMethod = convertMethod,
					OnSelected = delegate(ItemsControl container, SetSelectedInfo<T> info)
					{
						var treeItem = (TreeViewItem)container;
						treeItem.IsSelected = true;
						treeItem.BringIntoView();
					},
					OnNeedMoreItems = delegate(ItemsControl container, SetSelectedInfo<T> info)
					{
						((TreeViewItem)container).IsExpanded = true;
					}
				}
			);
		}
	}
	
	static class UIUtility
	{
		/// <summary>
		/// Selects an item in a hierarchial ItemsControl using a set of options
		/// </summary>
		/// <typeparam name="T">The type of the items present in the control and in the options</typeparam>
		/// <param name="control">The ItemsControl to select an item in</param>
		/// <param name="info">The options used for the selection process</param>
		public static void SetSelectedItem<T>(ItemsControl control, SetSelectedInfo<T> info)
		{
			var currentItem = info.Items.First();

			if (control.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
			{
				// Compare each item in the container and look for the next item
				// in the chain.
				foreach (object item in control.Items)
				{
					var convertedItem = default(T);

					// Convert the item if a conversion method exists. Otherwise
					// just cast the item to the desired type.
					if (info.ConvertMethod != null)
					{
						convertedItem = info.ConvertMethod(item);
					}
					else
					{
						convertedItem = (T)item;
					}

					// Compare the converted item with the item in the chain
					if ((info.CompareMethod != null) &&
						info.CompareMethod(convertedItem, currentItem))
					{
						var container = (ItemsControl)control.ItemContainerGenerator.ContainerFromItem(item);

						// Replace with the remaining items in the chain
						info.Items = info.Items.Skip(1);

						// If no items are left in the chain, then we're finished
						if (info.Items.Count() == 0)
						{
							// Select the last item
							if (info.OnSelected != null)
							{
								info.OnSelected(container, info);
							}
						}
						else
						{
							// Request more items and continue the search
							if (info.OnNeedMoreItems != null)
							{
								info.OnNeedMoreItems(container, info);
								SetSelectedItem<T>(container, info);
							}
						}

						break;
					}
				}
			}
			else
			{
				// If the item containers haven't been generated yet, attach an event
				// and wait for the status to change.
				EventHandler selectWhenReadyMethod = null;

				selectWhenReadyMethod = (ds, de) =>
				{
					if (control.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
					{
						// Stop listening for status changes on this container
						control.ItemContainerGenerator.StatusChanged -= selectWhenReadyMethod;

						// Search the container for the item chain
						SetSelectedItem(control, info);
					}
				};

				control.ItemContainerGenerator.StatusChanged += selectWhenReadyMethod;
			}
		}
	}

	class SetSelectedInfo<T>
	{
		/// <summary>
		/// Gets or sets the chain of items to search for. The last item in the chain will be selected.
		/// </summary>
		public IEnumerable<T> Items { get; set; }

		/// <summary>
		/// Gets or sets the method used to compare items in the control with items in the chain
		/// </summary>
		public Func<T, T, bool> CompareMethod { get; set; }

		/// <summary>
		/// Gets or sets the method used to convert items in the control to be compare with items in the chain
		/// </summary>
		public Func<object, T> ConvertMethod { get; set; }

		/// <summary>
		/// Gets or sets the method used to select the final item in the chain
		/// </summary>
		public SetSelectedEventHandler<T> OnSelected { get; set; }

		/// <summary>
		/// Gets or sets the method used to request more child items to be generated in the control
		/// </summary>
		public SetSelectedEventHandler<T> OnNeedMoreItems { get; set; }
	}

	delegate void SetSelectedEventHandler<T>(ItemsControl container, SetSelectedInfo<T> info);
}