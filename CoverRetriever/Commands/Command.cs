 



using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
namespace Microsoft.Practices.Prism.Commands
{
	/// <summary>
	/// Static Class that holds all Dependency Properties and Static methods to allow 
	/// the Activated event of the  Window class to be attached to a Command. 
	/// </summary>
	public static class Activated
	{
		private static readonly DependencyProperty ActivatedCommandBehaviorProperty = DependencyProperty.RegisterAttached(
			"ActivatedCommandBehavior",
			typeof(ActivatedCommandBehavior),
			typeof(Activated),
			null);


		/// <summary>
		/// Command to execute on click event.
		/// </summary>
		public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
			"Command",
			typeof(ICommand),
			typeof(Activated),
			new PropertyMetadata(OnSetCommandCallback));

		/// <summary>
		/// Command parameter to supply on command execution.
		/// </summary>
		public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
			"CommandParameter",
			typeof(object),
			typeof(Activated),
			new PropertyMetadata(OnSetCommandParameterCallback));


		/// <summary>
		/// Sets the <see cref="ICommand"/> to execute on the click event.
		/// </summary>
		/// <param name="control"> Window dependency object to attach command</param>
		/// <param name="command">Command to attach</param>
		public static void SetCommand(Window control, ICommand command)
		{
			control.SetValue(CommandProperty, command);
		}

		/// <summary>
		/// Retrieves the <see cref="ICommand"/> attached to the <see cref="ButtonBase"/>.
		/// </summary>
		/// <param name="control">Window containing the Command dependency property</param>
		/// <returns>The value of the command attached</returns>
		public static ICommand GetCommand(Window control)
		{
			return control.GetValue(CommandProperty) as ICommand;
		}

		/// <summary>
		/// Sets the value for the CommandParameter attached property on the provided <see cref="Window"/>.
		/// </summary>
		/// <param name="control">ButtonBase to attach CommandParameter</param>
		/// <param name="parameter">Parameter value to attach</param>
		public static void SetCommandParameter(Window control, object parameter)
		{
			control.SetValue(CommandParameterProperty, parameter);
		}

		/// <summary>
		/// Gets the value in CommandParameter attached property on the provided <see cref="ButtonBase"/>
		/// </summary>
		/// <param name="control">Window that has the CommandParameter</param>
		/// <returns>The value of the property</returns>
		public static object GetCommandParameter(Window control)
		{
			return control.GetValue(CommandParameterProperty);
		}

		private static void OnSetCommandCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			Window control = dependencyObject as Window;
			if (control != null)
			{
				var behavior = GetOrCreateBehavior(control);
				behavior.Command = e.NewValue as ICommand;
			}
		}

		private static void OnSetCommandParameterCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			Window control = dependencyObject as Window;
			if (control != null)
			{
				var behavior = GetOrCreateBehavior(control);
				behavior.CommandParameter = e.NewValue;
			}
		}

		private static ActivatedCommandBehavior GetOrCreateBehavior(Window control)
		{
			var behavior = control.GetValue(ActivatedCommandBehaviorProperty) as ActivatedCommandBehavior;
			if (behavior == null)
			{
				behavior = new ActivatedCommandBehavior(control);
				control.SetValue(ActivatedCommandBehaviorProperty, behavior);
			}

			return behavior;
		}
	}
	
	public class ActivatedCommandBehavior : CommandBehaviorBase<Window>
	{
		public ActivatedCommandBehavior(Window targetObject)
			: base(targetObject)
		{
			targetObject.Activated += OnActivated;
		}

		private void OnActivated(object sender, EventArgs routedEventArgs)
		{
			ExecuteCommand();
		}
	}
	/// <summary>
	/// Static Class that holds all Dependency Properties and Static methods to allow 
	/// the Deactivated event of the  Window class to be attached to a Command. 
	/// </summary>
	public static class Deactivated
	{
		private static readonly DependencyProperty DeactivatedCommandBehaviorProperty = DependencyProperty.RegisterAttached(
			"DeactivatedCommandBehavior",
			typeof(DeactivatedCommandBehavior),
			typeof(Deactivated),
			null);


		/// <summary>
		/// Command to execute on click event.
		/// </summary>
		public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
			"Command",
			typeof(ICommand),
			typeof(Deactivated),
			new PropertyMetadata(OnSetCommandCallback));

		/// <summary>
		/// Command parameter to supply on command execution.
		/// </summary>
		public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
			"CommandParameter",
			typeof(object),
			typeof(Deactivated),
			new PropertyMetadata(OnSetCommandParameterCallback));


		/// <summary>
		/// Sets the <see cref="ICommand"/> to execute on the click event.
		/// </summary>
		/// <param name="control"> Window dependency object to attach command</param>
		/// <param name="command">Command to attach</param>
		public static void SetCommand(Window control, ICommand command)
		{
			control.SetValue(CommandProperty, command);
		}

		/// <summary>
		/// Retrieves the <see cref="ICommand"/> attached to the <see cref="ButtonBase"/>.
		/// </summary>
		/// <param name="control">Window containing the Command dependency property</param>
		/// <returns>The value of the command attached</returns>
		public static ICommand GetCommand(Window control)
		{
			return control.GetValue(CommandProperty) as ICommand;
		}

		/// <summary>
		/// Sets the value for the CommandParameter attached property on the provided <see cref="Window"/>.
		/// </summary>
		/// <param name="control">ButtonBase to attach CommandParameter</param>
		/// <param name="parameter">Parameter value to attach</param>
		public static void SetCommandParameter(Window control, object parameter)
		{
			control.SetValue(CommandParameterProperty, parameter);
		}

		/// <summary>
		/// Gets the value in CommandParameter attached property on the provided <see cref="ButtonBase"/>
		/// </summary>
		/// <param name="control">Window that has the CommandParameter</param>
		/// <returns>The value of the property</returns>
		public static object GetCommandParameter(Window control)
		{
			return control.GetValue(CommandParameterProperty);
		}

		private static void OnSetCommandCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			Window control = dependencyObject as Window;
			if (control != null)
			{
				var behavior = GetOrCreateBehavior(control);
				behavior.Command = e.NewValue as ICommand;
			}
		}

		private static void OnSetCommandParameterCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			Window control = dependencyObject as Window;
			if (control != null)
			{
				var behavior = GetOrCreateBehavior(control);
				behavior.CommandParameter = e.NewValue;
			}
		}

		private static DeactivatedCommandBehavior GetOrCreateBehavior(Window control)
		{
			var behavior = control.GetValue(DeactivatedCommandBehaviorProperty) as DeactivatedCommandBehavior;
			if (behavior == null)
			{
				behavior = new DeactivatedCommandBehavior(control);
				control.SetValue(DeactivatedCommandBehaviorProperty, behavior);
			}

			return behavior;
		}
	}
	
	public class DeactivatedCommandBehavior : CommandBehaviorBase<Window>
	{
		public DeactivatedCommandBehavior(Window targetObject)
			: base(targetObject)
		{
			targetObject.Deactivated += OnDeactivated;
		}

		private void OnDeactivated(object sender, EventArgs routedEventArgs)
		{
			ExecuteCommand();
		}
	}
	/// <summary>
	/// Static Class that holds all Dependency Properties and Static methods to allow 
	/// the Loaded event of the  Window class to be attached to a Command. 
	/// </summary>
	public static class Loaded
	{
		private static readonly DependencyProperty LoadedCommandBehaviorProperty = DependencyProperty.RegisterAttached(
			"LoadedCommandBehavior",
			typeof(LoadedCommandBehavior),
			typeof(Loaded),
			null);


		/// <summary>
		/// Command to execute on click event.
		/// </summary>
		public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
			"Command",
			typeof(ICommand),
			typeof(Loaded),
			new PropertyMetadata(OnSetCommandCallback));

		/// <summary>
		/// Command parameter to supply on command execution.
		/// </summary>
		public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
			"CommandParameter",
			typeof(object),
			typeof(Loaded),
			new PropertyMetadata(OnSetCommandParameterCallback));


		/// <summary>
		/// Sets the <see cref="ICommand"/> to execute on the click event.
		/// </summary>
		/// <param name="control"> Window dependency object to attach command</param>
		/// <param name="command">Command to attach</param>
		public static void SetCommand(Window control, ICommand command)
		{
			control.SetValue(CommandProperty, command);
		}

		/// <summary>
		/// Retrieves the <see cref="ICommand"/> attached to the <see cref="ButtonBase"/>.
		/// </summary>
		/// <param name="control">Window containing the Command dependency property</param>
		/// <returns>The value of the command attached</returns>
		public static ICommand GetCommand(Window control)
		{
			return control.GetValue(CommandProperty) as ICommand;
		}

		/// <summary>
		/// Sets the value for the CommandParameter attached property on the provided <see cref="Window"/>.
		/// </summary>
		/// <param name="control">ButtonBase to attach CommandParameter</param>
		/// <param name="parameter">Parameter value to attach</param>
		public static void SetCommandParameter(Window control, object parameter)
		{
			control.SetValue(CommandParameterProperty, parameter);
		}

		/// <summary>
		/// Gets the value in CommandParameter attached property on the provided <see cref="ButtonBase"/>
		/// </summary>
		/// <param name="control">Window that has the CommandParameter</param>
		/// <returns>The value of the property</returns>
		public static object GetCommandParameter(Window control)
		{
			return control.GetValue(CommandParameterProperty);
		}

		private static void OnSetCommandCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			Window control = dependencyObject as Window;
			if (control != null)
			{
				var behavior = GetOrCreateBehavior(control);
				behavior.Command = e.NewValue as ICommand;
			}
		}

		private static void OnSetCommandParameterCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			Window control = dependencyObject as Window;
			if (control != null)
			{
				var behavior = GetOrCreateBehavior(control);
				behavior.CommandParameter = e.NewValue;
			}
		}

		private static LoadedCommandBehavior GetOrCreateBehavior(Window control)
		{
			var behavior = control.GetValue(LoadedCommandBehaviorProperty) as LoadedCommandBehavior;
			if (behavior == null)
			{
				behavior = new LoadedCommandBehavior(control);
				control.SetValue(LoadedCommandBehaviorProperty, behavior);
			}

			return behavior;
		}
	}
	
	public class LoadedCommandBehavior : CommandBehaviorBase<Window>
	{
		public LoadedCommandBehavior(Window targetObject)
			: base(targetObject)
		{
			targetObject.Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, EventArgs routedEventArgs)
		{
			ExecuteCommand();
		}
	}
	/// <summary>
	/// Static Class that holds all Dependency Properties and Static methods to allow 
	/// the Unloaded event of the  Window class to be attached to a Command. 
	/// </summary>
	public static class Unloaded
	{
		private static readonly DependencyProperty UnloadedCommandBehaviorProperty = DependencyProperty.RegisterAttached(
			"UnloadedCommandBehavior",
			typeof(UnloadedCommandBehavior),
			typeof(Unloaded),
			null);


		/// <summary>
		/// Command to execute on click event.
		/// </summary>
		public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
			"Command",
			typeof(ICommand),
			typeof(Unloaded),
			new PropertyMetadata(OnSetCommandCallback));

		/// <summary>
		/// Command parameter to supply on command execution.
		/// </summary>
		public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
			"CommandParameter",
			typeof(object),
			typeof(Unloaded),
			new PropertyMetadata(OnSetCommandParameterCallback));


		/// <summary>
		/// Sets the <see cref="ICommand"/> to execute on the click event.
		/// </summary>
		/// <param name="control"> Window dependency object to attach command</param>
		/// <param name="command">Command to attach</param>
		public static void SetCommand(Window control, ICommand command)
		{
			control.SetValue(CommandProperty, command);
		}

		/// <summary>
		/// Retrieves the <see cref="ICommand"/> attached to the <see cref="ButtonBase"/>.
		/// </summary>
		/// <param name="control">Window containing the Command dependency property</param>
		/// <returns>The value of the command attached</returns>
		public static ICommand GetCommand(Window control)
		{
			return control.GetValue(CommandProperty) as ICommand;
		}

		/// <summary>
		/// Sets the value for the CommandParameter attached property on the provided <see cref="Window"/>.
		/// </summary>
		/// <param name="control">ButtonBase to attach CommandParameter</param>
		/// <param name="parameter">Parameter value to attach</param>
		public static void SetCommandParameter(Window control, object parameter)
		{
			control.SetValue(CommandParameterProperty, parameter);
		}

		/// <summary>
		/// Gets the value in CommandParameter attached property on the provided <see cref="ButtonBase"/>
		/// </summary>
		/// <param name="control">Window that has the CommandParameter</param>
		/// <returns>The value of the property</returns>
		public static object GetCommandParameter(Window control)
		{
			return control.GetValue(CommandParameterProperty);
		}

		private static void OnSetCommandCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			Window control = dependencyObject as Window;
			if (control != null)
			{
				var behavior = GetOrCreateBehavior(control);
				behavior.Command = e.NewValue as ICommand;
			}
		}

		private static void OnSetCommandParameterCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			Window control = dependencyObject as Window;
			if (control != null)
			{
				var behavior = GetOrCreateBehavior(control);
				behavior.CommandParameter = e.NewValue;
			}
		}

		private static UnloadedCommandBehavior GetOrCreateBehavior(Window control)
		{
			var behavior = control.GetValue(UnloadedCommandBehaviorProperty) as UnloadedCommandBehavior;
			if (behavior == null)
			{
				behavior = new UnloadedCommandBehavior(control);
				control.SetValue(UnloadedCommandBehaviorProperty, behavior);
			}

			return behavior;
		}
	}
	
	public class UnloadedCommandBehavior : CommandBehaviorBase<Window>
	{
		public UnloadedCommandBehavior(Window targetObject)
			: base(targetObject)
		{
			targetObject.Unloaded += OnUnloaded;
		}

		private void OnUnloaded(object sender, EventArgs routedEventArgs)
		{
			ExecuteCommand();
		}
	}
	/// <summary>
	/// Static Class that holds all Dependency Properties and Static methods to allow 
	/// the MouseDoubleClick event of the  Control class to be attached to a Command. 
	/// </summary>
	public static class MouseDoubleClick
	{
		private static readonly DependencyProperty MouseDoubleClickCommandBehaviorProperty = DependencyProperty.RegisterAttached(
			"MouseDoubleClickCommandBehavior",
			typeof(MouseDoubleClickCommandBehavior),
			typeof(MouseDoubleClick),
			null);


		/// <summary>
		/// Command to execute on click event.
		/// </summary>
		public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
			"Command",
			typeof(ICommand),
			typeof(MouseDoubleClick),
			new PropertyMetadata(OnSetCommandCallback));

		/// <summary>
		/// Command parameter to supply on command execution.
		/// </summary>
		public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
			"CommandParameter",
			typeof(object),
			typeof(MouseDoubleClick),
			new PropertyMetadata(OnSetCommandParameterCallback));


		/// <summary>
		/// Sets the <see cref="ICommand"/> to execute on the click event.
		/// </summary>
		/// <param name="control"> Control dependency object to attach command</param>
		/// <param name="command">Command to attach</param>
		public static void SetCommand(Control control, ICommand command)
		{
			control.SetValue(CommandProperty, command);
		}

		/// <summary>
		/// Retrieves the <see cref="ICommand"/> attached to the <see cref="ButtonBase"/>.
		/// </summary>
		/// <param name="control">Control containing the Command dependency property</param>
		/// <returns>The value of the command attached</returns>
		public static ICommand GetCommand(Control control)
		{
			return control.GetValue(CommandProperty) as ICommand;
		}

		/// <summary>
		/// Sets the value for the CommandParameter attached property on the provided <see cref="Control"/>.
		/// </summary>
		/// <param name="control">ButtonBase to attach CommandParameter</param>
		/// <param name="parameter">Parameter value to attach</param>
		public static void SetCommandParameter(Control control, object parameter)
		{
			control.SetValue(CommandParameterProperty, parameter);
		}

		/// <summary>
		/// Gets the value in CommandParameter attached property on the provided <see cref="ButtonBase"/>
		/// </summary>
		/// <param name="control">Control that has the CommandParameter</param>
		/// <returns>The value of the property</returns>
		public static object GetCommandParameter(Control control)
		{
			return control.GetValue(CommandParameterProperty);
		}

		private static void OnSetCommandCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			Control control = dependencyObject as Control;
			if (control != null)
			{
				var behavior = GetOrCreateBehavior(control);
				behavior.Command = e.NewValue as ICommand;
			}
		}

		private static void OnSetCommandParameterCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			Control control = dependencyObject as Control;
			if (control != null)
			{
				var behavior = GetOrCreateBehavior(control);
				behavior.CommandParameter = e.NewValue;
			}
		}

		private static MouseDoubleClickCommandBehavior GetOrCreateBehavior(Control control)
		{
			var behavior = control.GetValue(MouseDoubleClickCommandBehaviorProperty) as MouseDoubleClickCommandBehavior;
			if (behavior == null)
			{
				behavior = new MouseDoubleClickCommandBehavior(control);
				control.SetValue(MouseDoubleClickCommandBehaviorProperty, behavior);
			}

			return behavior;
		}
	}
	
	public class MouseDoubleClickCommandBehavior : CommandBehaviorBase<Control>
	{
		public MouseDoubleClickCommandBehavior(Control targetObject)
			: base(targetObject)
		{
			targetObject.MouseDoubleClick += OnMouseDoubleClick;
		}

		private void OnMouseDoubleClick(object sender, EventArgs routedEventArgs)
		{
			ExecuteCommand();
		}
	}
	/// <summary>
	/// Static Class that holds all Dependency Properties and Static methods to allow 
	/// the SelectedItemChanged event of the  TreeView class to be attached to a Command. 
	/// </summary>
	public static class SelectedItemChanged
	{
		private static readonly DependencyProperty SelectedItemChangedCommandBehaviorProperty = DependencyProperty.RegisterAttached(
			"SelectedItemChangedCommandBehavior",
			typeof(SelectedItemChangedCommandBehavior),
			typeof(SelectedItemChanged),
			null);


		/// <summary>
		/// Command to execute on click event.
		/// </summary>
		public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
			"Command",
			typeof(ICommand),
			typeof(SelectedItemChanged),
			new PropertyMetadata(OnSetCommandCallback));

		/// <summary>
		/// Command parameter to supply on command execution.
		/// </summary>
		public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
			"CommandParameter",
			typeof(object),
			typeof(SelectedItemChanged),
			new PropertyMetadata(OnSetCommandParameterCallback));


		/// <summary>
		/// Sets the <see cref="ICommand"/> to execute on the click event.
		/// </summary>
		/// <param name="control"> TreeView dependency object to attach command</param>
		/// <param name="command">Command to attach</param>
		public static void SetCommand(TreeView control, ICommand command)
		{
			control.SetValue(CommandProperty, command);
		}

		/// <summary>
		/// Retrieves the <see cref="ICommand"/> attached to the <see cref="ButtonBase"/>.
		/// </summary>
		/// <param name="control">TreeView containing the Command dependency property</param>
		/// <returns>The value of the command attached</returns>
		public static ICommand GetCommand(TreeView control)
		{
			return control.GetValue(CommandProperty) as ICommand;
		}

		/// <summary>
		/// Sets the value for the CommandParameter attached property on the provided <see cref="TreeView"/>.
		/// </summary>
		/// <param name="control">ButtonBase to attach CommandParameter</param>
		/// <param name="parameter">Parameter value to attach</param>
		public static void SetCommandParameter(TreeView control, object parameter)
		{
			control.SetValue(CommandParameterProperty, parameter);
		}

		/// <summary>
		/// Gets the value in CommandParameter attached property on the provided <see cref="ButtonBase"/>
		/// </summary>
		/// <param name="control">TreeView that has the CommandParameter</param>
		/// <returns>The value of the property</returns>
		public static object GetCommandParameter(TreeView control)
		{
			return control.GetValue(CommandParameterProperty);
		}

		private static void OnSetCommandCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			TreeView control = dependencyObject as TreeView;
			if (control != null)
			{
				var behavior = GetOrCreateBehavior(control);
				behavior.Command = e.NewValue as ICommand;
			}
		}

		private static void OnSetCommandParameterCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			TreeView control = dependencyObject as TreeView;
			if (control != null)
			{
				var behavior = GetOrCreateBehavior(control);
				behavior.CommandParameter = e.NewValue;
			}
		}

		private static SelectedItemChangedCommandBehavior GetOrCreateBehavior(TreeView control)
		{
			var behavior = control.GetValue(SelectedItemChangedCommandBehaviorProperty) as SelectedItemChangedCommandBehavior;
			if (behavior == null)
			{
				behavior = new SelectedItemChangedCommandBehavior(control);
				control.SetValue(SelectedItemChangedCommandBehaviorProperty, behavior);
			}

			return behavior;
		}
	}
	
	public class SelectedItemChangedCommandBehavior : CommandBehaviorBase<TreeView>
	{
		public SelectedItemChangedCommandBehavior(TreeView targetObject)
			: base(targetObject)
		{
			targetObject.SelectedItemChanged += OnSelectedItemChanged;
		}

		private void OnSelectedItemChanged(object sender, EventArgs routedEventArgs)
		{
			ExecuteCommand();
		}
	}
}