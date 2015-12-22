# ControlAndInteractivity
http://www.codeproject.com/Articles/1056014/WPF-Lookless-Controls?msg=5177531#xx5177531xx


WPF Lookless Controls
Nick Polyak, 19 Nov 2015 CPOL
	   4.89 (31 votes)
	
Rate: 	
vote 1vote 2vote 3vote 4vote 5
	
Lookless controls vs User Controls. Lookless controls usage patterns

    Download source - 153.5 KB

Introduction

I work for a company that started using WPF relatively recently. As a result, a lot of software engineers are switching to programming C# WPF from other platforms and languages. The easiest and most intuitive way of creating a re-usable visual control in WPF is by utilizing the built in VS2015 template for UserControl.

The purpose of this article to show that Lookless controls are actually a better choice than UserControls. As will be shown below, the Lookless controls provide considerably more flexibility and separation of concerns.

I will also discuss the best practices and re-use patterns when it comes to Lookless Controls.

Reader 'webmaster442' noticed, most WPF literature calls Lookless controls - Custom controls. I wanted to avoid using this name for two reasons:

    Some readers might be confused by the name and think that User Controls are also custom controls because they can be custom made.
    Visual Studio has a template for Custom Controls which is pretty much useless and confusing and does not serve any purpose from my point of view. So I do not want to encourage the readers to use it.

This article is for people who are starting to work with WPF and are curious about ways to improve WPF coding practices.

Here are the topics covered in this article

    User Control Example
    The Drawbacks of UserControls
    A Lookless Control Example
    Using Two Different Templates for the same Lookless Control
    Parametrizing Lookless Controls
    Accessing the Visuals for C# Code for Lookless Controls
    Summary
    Appendix A. Creating a UserControl in Visual Studio
    Appending B. Installing a Custom propdp Snippet
    Appending C: Creating a WPF Resource Dictionary File

An UserControl Example

In WPF (and other frameworks) the controls are created in order to encapsulate some parts of re-usable UI functionality.

Let us start by showing an example of creating and (re)using a WPF UserControl.

The code for this example can be found under UserControlSample project.

Running this project, you'll see the following window:

There are two rows in the window - one for entering the name and the other for entering the passcode.

Each of the rows consists of a label (non-editable text), an editable text box and an 'X' button, clicking which clears the corresponding text box.

The two rows for entering the name and the passcode are represented by two very simple instances of the same EditableTextAndLabelUserControl class.

Here is the MainWindow.xaml code that creates the window:
Hide   Copy Code

<Window x:Class="UserControlSample.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:local="clr-namespace:UserControlSample"
        Title="MainWindow"
        Height="350"
        Width="525">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="30"/>
            <RowDefinition Height="30"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <!-- user control for entering the name -->
        <local:EditableTextAndLabelUserControl Label="Enter your name:" 
                                               HorizontalAlignment="Left"
                                               VerticalAlignment="Center"
                                               Margin="10,0,0,0"/>
        
        <!-- user control for entering the passcode -->
        <local:EditableTextAndLabelUserControl Label="Enter your passcode:"
                                               HorizontalAlignment="Left"
                                               VerticalAlignment="Center" 
                                               Margin="10,0,0,0"
                                               Grid.Row="1"/>
    </Grid>
</Window>

The code for EditableTextAndLabelUserControl is also part of the same project contained within XAML file EditableTextAndLabelUserControl.xaml and C# file EditableTextAndLabelUserControl.xaml.cs. (For instructions on how to create a UserControl in Visual Studio, please, check the Appendix A: Creating a UserControl.)

EditableTextAndLabelUserControl.xaml file contains the following simple XAML:
Hide   Copy Code

<UserControl x:Class="UserControlSample.EditableTextAndLabelUserControl"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="clr-namespace:UserControlSample">
    <StackPanel Orientation="Horizontal"
                VerticalAlignment="Stretch">
        <TextBlock x:Name="TheLabel"
                   Margin="0,0,2,0"/>
        <TextBox x:Name="TheTextBox"
                 Width="100"
                 Margin="2,0"/>
        <Button x:Name="TheClearButton"
                Content="X"
                Width="30"/>
    </StackPanel>
</UserControl>

The label is represented by TextBlock named TheLabel; the editable text is represented by TextBox named TheTextBox and the button is represented by Button control named TheClearButton (I like prepending XAML names by prefix "The" in order to distinguish them from the type names).

The WPF XAML parsing engine creates the variables corresponding to the control names. These variables are accessible from the code behind (in our case contained within EditableTextAndLabelUserControl.xaml.cs file. This code is also very simple:
Hide   Shrink   Copy Code

public partial class EditableTextAndLabelUserControl : UserControl
{
    public EditableTextAndLabelUserControl()
    {
        InitializeComponent();

        TheClearButton.Click += TheClearButton_Click;
    }

    private void TheClearButton_Click(object sender, RoutedEventArgs e)
    {
        Clear();
    }

    public void Clear()
    {
        EditableText = null;
    }

    public string Label
    {
        get
        {
            return TheLabel.Text;
        }

        set
        {
            TheLabel.Text = value;
        }
    }

    public string EditableText
    {
        get
        {
            return TheTextBox.Text;
        }

        set
        {
            TheTextBox.Text = value;
        }
    }
}  

It provides the public string properties Label and EditableText as a means of communicating with the rest of the application and a method Clear() invoked on the 'X' button click (this invocation is wired via the button's click event handler).
The Drawbacks of UserControls

As simple and powerful as UserControls can be, they have an inherent shortcoming - the visual representation is permanently tied to the C# code. The visual representation of EditableTextAndLabelUserControl control discussed above, cannot be changed - it will always remain a TextBlock followed by a TextBox followed by a Button arranged horizontally one after another. To be fair - some extra degree of freedom for UserControls can be achieved by parametrization (in the same way as will be discussed below for Lookless Controls), but their core visual representation is fixed and tied forever to the C# code behind.

The Lookless Controls completely overcome these limitation. They usually consist of a number of non-visual methods and properties and do not impose any constraints on the visual implementation. The visual implementation for the Lookless Controls is provided by XAML templates and styles and can be changed very easily without ANY changes to the control code itself or to the way the Lookless Control interacts with the rest of the application.
A Lookless Control Example

Code for the simple Lookless Control example is located under LooklessControlSample project. When it runs it shows exactly the same Window as the UserControl example above.

Take a look at EditableTextAndLabelControl class located within EditableTextAndLabelControl.cs file. (Note that it is a simple C# file - no special VS templates are necessary to create it).

The class derives from System.Windows.Controls.Control class and it contains two string dependency properties Label and EditableText as well as a method Clear (which simply sets EditableText property to null).
Hide   Shrink   Copy Code

public class EditableTextAndLabelControl : Control
{
    #region Label Dependency Property
    public string Label
    {
        get { return (string)GetValue(LabelProperty); }
        set { SetValue(LabelProperty, value); }
    }

    public static readonly DependencyProperty LabelProperty =
    DependencyProperty.Register
    (
        "Label",
        typeof(string),
        typeof(EditableTextAndLabelControl),
        new PropertyMetadata(null)
    );
    #endregion Label Dependency Property


    #region EditableText Dependency Property
    public string EditableText
    {
        get { return (string)GetValue(EditableTextProperty); }
        set { SetValue(EditableTextProperty, value); }
    }

    public static readonly DependencyProperty EditableTextProperty =
    DependencyProperty.Register
    (
        "EditableText",
        typeof(string),
        typeof(EditableTextAndLabelControl),
        new PropertyMetadata(null)
    );
    #endregion EditableText Dependency Property


    public void Clear()
    {
        this.EditableText = null;
    }
}  

The dependency properties code may look like a terrible mess for people who are not used to them, but they can be very easily created with propdp snippet that is built into the Visual Studio, by typing propdp and then providing the name, the type and the default value of the dependency property. You tab in order to switch between the dependency property parts and press 'enter' after you are done.

My propdp snippet (located under CODE/Snippets folder) is a slightly improved version of the one that comes with the Visual Studio. It arranges the code more vertically, provides the container class name automatically and creates a region for every dependency property. This snippet can be copied into the your NetFX30 snippet folder to replace the default propdp snippet. For detailed instructions on installing the snippet, please, look at Appendix B: Installing a Custom propdp Snippet.

Once you define a dependency property you can use it as if it is a normal property with some extra features, in particular dependency property:

    Provide change notifications that WPF bindings understand and thus can serve as WPF binding's source property.
    Can be used as WPF binding target property (the usual properties cannot be used for that).
    Can be modified by WPF Styles.
    Can be animated.

 

We are mostly interested in the fact that dependency properties can server as the source and the target properties of the WPF bindings. This is very important for the control to be able to communicate with the rest of the application.

For simplicity sake we placed the ControlTemplate for our Lookless Control into the MainWindow.xaml file - the Window.Resources section:
Hide   Shrink   Copy Code

<ControlTemplate x:Key="PlainHorizontalEditableTextTemplate"
                 TargetType="local:EditableTextAndLabelControl">
    <StackPanel Orientation="Horizontal"
                VerticalAlignment="Stretch">
        <!-- bound to the Label dependency property of the control -->
        <TextBlock x:Name="TheLabel"
                   Text="{Binding Path=Label,
                                  RelativeSource={RelativeSource TemplatedParent}}"
                   Margin="0,0,2,0" />
        <!-- bound to the EditableText dependency property of the control -->
        <TextBox x:Name="TheTextBox"
                 Text="{Binding Path=EditableText,
                                Mode=TwoWay,
                                RelativeSource={RelativeSource TemplatedParent}}"
                 Width="100"
                 Margin="2,0" />

        <!-- We use Expression Blend's SDK's EventTrigger and CallMethodAction
             objects to wire Clear() method of the control 
             to the Click event of the button -->
        <Button x:Name="TheClearButton"
                Content="X"
                Width="30">
            <i:Interaction.Triggers>
                <i:EventTrigger EventName="Click">
                    <ei:CallMethodAction MethodName="Clear"
                                         TargetObject="{Binding RelativeSource={RelativeSource TemplatedParent}}"/>
                </i:EventTrigger>
            </i:Interaction.Triggers>
        </Button>
    </StackPanel>
</ControlTemplate>

Note that we are using bindings with RelativeSource set to TemplatedParent mode, e.g:
Hide   Copy Code

<TextBox x:Name="TheTextBox"
     Text="{Binding Path=EditableText,
                    Mode=TwoWay,
                    RelativeSource={RelativeSource TemplatedParent}}"  
.../>

The RelativeSource TemplatedParent instructs the binding to search for the source property on the control whose control template this XAML code belongs to - in our case, it is the EditableTextAndLabelControl.

There is a shorthand for TemplatedParent binding - instead of writing all this binding XAML above, we could have simply written
Hide   Copy Code

<TextBox x:Name="TheTextBox"
     Text="{TemplateBinding EditableText}"  
.../>  

I repeat, the binding above works only because we are programming a control template for a control. A more generic way would be to find the source object of the binding by type using AncestorType property of RelativeSource instead of TemplatedParent:
Hide   Copy Code

<TextBox x:Name="TheTextBox"
     Text="{Binding Path=EditableText,
                    Mode=TwoWay,
                    RelativeSource={RelativeSource AncestorType=local:EditableTextAndLabelControl}}"  
.../>

The code above is more generic because it would work anywhere under the visual tree from our EditableTextAndLabelControl, not only in the immediate template. This might be useful if one tries to maximize the XAML re-use by using sub-controls with sub-templates or if one uses DataTemplates instead of ControlTemplate (both topics are beyond the scope of this article).

The Button's Click event is wired to the Clear() method of the EditableTextAndLabelControl via Expression Blend SDK functionality:
Hide   Copy Code

<Button x:Name="TheClearButton"
        Content="X"
        Width="30">
    <i:Interaction.Triggers>
        <i:EventTrigger EventName="Click">
            <ei:CallMethodAction MethodName="Clear"
                                 TargetObject="{Binding RelativeSource={RelativeSource TemplatedParent}}" />
        </i:EventTrigger>
    </i:Interaction.Triggers>
</Button>  

You do not have to install MS Expression Blend in order to get this functionality. All you need to do is to reference two dlls that can be found under CODE/ExpressionBlenSDK folder: Microsoft.Expression.Interactions.dll and System.Windows.Interactivity.dll.

You also need to set references to two namespaces in your XAML file header:
Hide   Copy Code

xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity"
xmlns:ei="http://schemas.microsoft.com/expression/2010/interactions"  

As shown above, this functionality allows to hook to any routed event of any control and call any method on any bindable objects whenever the event fires.

A more common way of wiring an event to C# functionality is by using WPF Commands. Commands, however, are less flexible and more cumbersome for the following reasons:

    Commands can only be fired for the Click event of very few controls, i.e. Buttons and Menus while the Expression Blend SDK functionality can used for any Routed Event on any control.
    Commands usually require adding them to the C# code, while the Expression Blend SDK functionality can call any method within the C# code as long as the method has no arguments or if its arguments are the same as those of the routed events.

 

Note: The Expression Blend SDK functionality comes with many other popular packages, e.g. if you are using Prism you should already have the required Expression Blend SDK dlls.

The MainWindow's code is very simple and very similar to that of the previous example:
Hide   Copy Code

<!-- user control for entering the name -->
<local:EditableTextAndLabelControl Label="Enter your name:"
                                   HorizontalAlignment="Left"
                                   VerticalAlignment="Center"
                                   Margin="10,0,0,0"
                                   Template="{StaticResource PlainHorizontalEditableTextTemplate}"/>

<!-- user control for entering the passcode -->
<local:EditableTextAndLabelControl Label="Enter your passcode:"
                                   HorizontalAlignment="Left"
                                   VerticalAlignment="Center"
                                   Margin="10,0,0,0"
                                   Grid.Row="1"
                                   Template="{StaticResource PlainHorizontalEditableTextTemplate}"/>  

Note the references to the ControlTemplate via the StaticResource markup extension:
Hide   Copy Code

Template="{StaticResource PlainHorizontalEditableTextTemplate}"  

Note that the XAML code for both controls is very similar: we are using the same HorizontalAlignment, VerticalAlignment, Margin and Template properties for both of them. In order to re-use more XAML code we can use a WPF Style that sets those properties:
Hide   Copy Code

<Style x:Key="TheEditableTextAndLabelControlStyle"
       TargetType="local:EditableTextAndLabelControl">
    <Setter Property="HorizontalAlignment"
            Value="Left"/>
    <Setter Property="VerticalAlignment"
            Value="Center"/>
    <Setter Property="Margin"
            Value="10,0,0,0"/>
    <Setter Property="Template"
            Value="{StaticResource PlainHorizontalEditableTextTemplate}"/>
</Style>  

This style should be placed in the Windows.Resources section of MainWindow.xaml file under the Template's definition (because the style refers to the template).

Then, we can remove the properties defined in the Style from the controls, instead making the controls refer to the style in the following way:
Hide   Copy Code

<!-- user control for entering the name -->
<local:EditableTextAndLabelControl Label="Enter your name:"
                                   Template="{StaticResource PlainHorizontalEditableTextTemplate}"
                                   Style="{StaticResource TheEditableTextAndLabelControlStyle}"/>

<!-- user control for entering the passcode -->
<local:EditableTextAndLabelControl Label="Enter your passcode:"
                                   Grid.Row="1"
                                   Style="{StaticResource TheEditableTextAndLabelControlStyle}" />  

To even further simplify XAML, we can make the Style to be the default style for all EditableTextAndLabelControl by not setting the x:Key property on it. In that case we can even drop the reference to such Style from the controls:
Hide   Copy Code

<!-- user control for entering the name -->
<local:EditableTextAndLabelControl Label="Enter your name:"
                                   Template="{StaticResource PlainHorizontalEditableTextTemplate}"/>

<!-- user control for entering the passcode -->
<local:EditableTextAndLabelControl Label="Enter your passcode:"
                                   Grid.Row="1">   

Finally, the best practice is to place the Styles and Templates into a separate ResourceDictionary file, usually located in a separate folder, so that they could be accessed from multiple places. In the further projects we will have a folder "Styles" containing the ResourceDictionarys for various controls.
Using Two Different Templates for the same Lookless Control

In the next sample I show how to create two completely different templates for the same lookless control. The sample is located under TwoDifferentTemplatesForTheSameLooklessControlSample project.

Here is what you see if you run the project:

Both, on the left and right I am displaying the same EditableTextAndLabelControl control, but on the left I am using PlainHorizontalEditableTextStyle Style and on the right I am using SomeCrazyEditableTextStyle Style:
Hide   Copy Code

<Grid>
    ...
    <local:EditableTextAndLabelControl Label="Enter your name:"
                                       Style="{StaticResource PlainHorizontalEditableTextStyle}"
                                       Grid.Row="1"/>

    ...

    <local:EditableTextAndLabelControl Label="Enter your name:"
                                       Style="{StaticResource SomeCrazyEditableTextStyle}"
                                       Grid.Column="2"
                                       Grid.Row="1"/>
</Grid>  

Both Styles are defined in LooklessControlStyles.xaml Resource Dictionary file located under "Styles" project folder:

In order to create a Resource Dictionary, please follow the steps specified in Appending C: Creating a WPF ResourceDictioary File.

LooklessControlStyles.xaml Resource Dictionary defines two templates: "PlainHorizontalEditableTextTemplate" (same as in the previous example) and "SomeCrazyEditableTextTemplate" (the new one).

It also has two styles: "PlainHorizontalEditableTextStyle" and "SomeCrazyEditableTextStyle" that use these two templates correspondingly.

We are more interested in the "Crazy" Style and Template since the "Plain" ones have been described before.

Here is the XAML of the "Crazy" Style:
Hide   Copy Code

<Style TargetType="local:EditableTextAndLabelControl"
       x:Key="SomeCrazyEditableTextStyle">
    <Setter Property="HorizontalAlignment"
            Value="Left" />
    <Setter Property="VerticalAlignment"
            Value="Center" />
    <Setter Property="Margin"
            Value="10,0,0,0" />
    <Setter Property="Template"
            Value="{StaticResource SomeCrazyEditableTextTemplate}" />
</Style>  

And here is the "Crazy" Template:
Hide   Shrink   Copy Code

<ControlTemplate x:Key="SomeCrazyEditableTextTemplate"
                 TargetType="local:EditableTextAndLabelControl">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition />
            <RowDefinition />
        </Grid.RowDefinitions>
        <Grid.ColumnDefinitions>
            <ColumnDefinition />
            <ColumnDefinition />
        </Grid.ColumnDefinitions>
        <!-- bound to the Label dependency property of the control -->
        <Label x:Name="TheLabel"
               Content="{Binding Path=Label,
                                    RelativeSource={RelativeSource TemplatedParent}}"
               Margin="0,0,2,0"
               Background="Red"
               Grid.Row="1"
               RenderTransformOrigin="0.5,0.5">
            <Label.RenderTransform>
                <RotateTransform Angle="-45" />
            </Label.RenderTransform>
        </Label>
        <!-- bound to the EditableText dependency property of the control -->
        <TextBox x:Name="TheTextBox"
                 Text="{Binding Path=EditableText,
                                Mode=TwoWay,
                                RelativeSource={RelativeSource TemplatedParent}}"
                 Width="100"
                 Margin="2,0"
                 Grid.Column="1" />

        <!-- We use Expression Blend's SDK's EventTrigger and CallMethodAction
                 objects to wire Clear() method of the control 
                 to the Click event of the button -->
        <ToggleButton x:Name="TheClearButton"
                      Content="X"
                      Width="30"
                      Grid.Column="2"
                      Grid.Row="1">
            <i:Interaction.Triggers>
                <i:EventTrigger EventName="Click">
                    <ei:CallMethodAction MethodName="Clear"
                                         TargetObject="{Binding RelativeSource={RelativeSource TemplatedParent}}" />
                </i:EventTrigger>
            </i:Interaction.Triggers>
        </ToggleButton>
    </Grid>
</ControlTemplate>  

Note, that not only we arrange the controls differently, but we are using different controls - e.g. instead of TextBlock we are using Label control and instead of Button, we are using ToggleButton.

Also note, that in order to make the Styles and Templates defined in a separate Resource Dictionary file visible within the MainWindow.xaml, we added the following code to its Window.Resources section:
Hide   Copy Code

<Window.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="Styles/LooklessControlStyles.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Window.Resources>  

Often, the referred Resource Dictionary will be in a different project. In that case, the Source property of the merged resource dictionary should be set differently, e.g. assuming that we refer to "Styles/LooklessControlStyles.xaml" in a different project called "Controls" here is how the corresponding line would look:
Hide   Copy Code

<ResourceDictionary Source="/Controls;Component/Styles/LooklessControlStyles.xaml"/> 

Note, that we also added prefix "Component" to the corresponding path within the project.
Parametrizing Lookless Controls

We can further improve the re-use of XAML by making it dependent on some dependency properties of the Lookless Control.

If you run ParametrizingLooklessControl project, and start typing the text in its TextBox you will see that the label text is blue, while the editable text is red:

Here is the relevant content of the MainWindow.xaml file
Hide   Copy Code

<local:EditableTextAndLabelControl Label="Enter your name:"
                                   Foreground="Blue"
                                   EditableTextForeground="Red"
                                   Style="{StaticResource PlainHorizontalEditableTextStyle}"/>  

You can see, that Foreground property of the control is set to "Blue", while the property EditableTextForeground is set to "Red".

Every WPF Control has the Foreground property, so, since EditableTextAndLabelControl class derives from Control it gets the Foreground property automatically.

The dependency property EditableTextForeground has been, however, added to the EditableTextAndLabelControl class:
Hide   Copy Code

#region EditableTextForeground Dependency Property
public Brush EditableTextForeground
{
    get { return (Brush)GetValue(EditableTextForegroundProperty); }
    set { SetValue(EditableTextForegroundProperty, value); }
}

public static readonly DependencyProperty EditableTextForegroundProperty =
DependencyProperty.Register
(
    "EditableTextForeground",
    typeof(Brush),
    typeof(EditableTextAndLabelControl),
    new PropertyMetadata(null)
);
#endregion EditableTextForeground Dependency Property  

The Template (defined in LooklessControlStyles.xaml file) binds the corresponding TextBlock's and TextBox'es Foreground properties to the Foreground and EditableTextForeground properties of the Control in the following way:
Hide   Copy Code

<!-- bound to the Label dependency property of the control -->
<TextBlock x:Name="TheLabel"
           Foreground="{Binding Path=Foreground,
                                RelativeSource={RelativeSource TemplatedParent}}"
           Text="{Binding Path=Label,
                          RelativeSource={RelativeSource TemplatedParent}}"
           Margin="0,0,2,0" />
<!-- bound to the EditableText dependency property of the control -->
<TextBox x:Name="TheTextBox"
         Foreground="{Binding Path=EditableTextForeground,
                              RelativeSource={RelativeSource TemplatedParent}}"
         Text="{Binding Path=EditableText,
                            Mode=TwoWay,
                            RelativeSource={RelativeSource TemplatedParent}}"
         Width="100"
         Margin="2,0" />  

Accessing the Visuals From C# Code for Lookless Controls

Sometimes (though not very often) it is necessary to access the parts defined in a Control Template from the C# code of the control. This, BTW is the only small advantage that UserControls have over Lookless Controls - it is easier to access the visuals defined in XAML for UserControls from C# code.

There is, however, a relatively simple way of accessing visual parts for Lookless Controls also.

Project AccessingVisualPartsFromCSharpSample shows how it can be done.

If you run the project and click on "Enter your name:" label, you will see a modal dialog window containing text "label clicked" popping up:

In order to continue working with the main window, you'll have to kill the popup first.

Take a look at the Control Template within LooklessControlStyle.xaml file. The only things that changed in comparison to the previous samples is that the TextBlock has been renamed "PART_Label" and it has a transparent background:
Hide   Copy Code

<TextBlock x:Name="PART_TheLabel"
           Background="Transparent"
           Text="{Binding Path=Label,
                          RelativeSource={RelativeSource TemplatedParent}}"
           Margin="0,0,2,0" />

Prefix "PART_" for the names of the visuals that might be accessed from within C# code is a WPF useful common practice.

Transparency for the background of the TextBlock is introduced in order to better catch the mouse down events on it (otherwise it would only react when one clicks on the text itself)

The relevant C# code addition is located at the top of EditableTextAndLabelControl.cs file:
Hide   Copy Code

public class EditableTextAndLabelControl : Control
{
    FrameworkElement _labelElement = null;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // find _labelElement from the control template
        _labelElement = 
            this.Template.FindName("PART_TheLabel", this) as FrameworkElement;

        // attach event handler to MouseLeftButtonDown event.
        _labelElement.MouseLeftButtonDown += _labelElement_MouseLeftButtonDown;
    }

    private void _labelElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // create and show a modal window with text "label clicked!"
        Window dialogWindow = new Window() { Width = 150, Height = 100 };

        dialogWindow.Content =
            new TextBox { Text = "label clicked!" };
        dialogWindow.ShowDialog();
    }
...
}

We override OnApplyTemplate() method (to make sure that we can find the parts defined in the control template).

Then, we use Template.FindName(...) method to actually find the control.

Other and more powerful ways of finding template parts is to use functionality described in Generic (Non-WPF) Tree to LINQ and Event Propagation on Trees. This approach will be described elsewhere.
Summary

In this article I explain why WPF Lookless Controls are more powerful that User Controls and describe several usage patterns for the Lookless Controls.
History of Changes

    Nov. 16, 2015 - added a table of contents; explained my using the term 'Lookless control' instead of 'Custom controls'
    Nov 19, 2015 - added a missing link to the source code

Appedix A: Creating a UserControl in Visual Studio

In order to create a WPF UserControl in Visual Studio, right mouse click on the project in Solution Explorer; choose Add->New Item; then choose WPF on the left hand side and 'User Control (WPF)' on the right hand side and enter the name of the control:

 

Appending B: Installing a Custom propdp Snippet

You can install the custom propdp snippet that comes with the code (in CODE/Snippets folder) by going through the following steps:

    Open the Snippets Manager by going to the "Tools" menu in Visual Studio and clicking on "Code Snippets Manager" menu item:
    Within the Snippets Manager, change the language to CSharp (using the drop down at the top) and click on "NetFX30" folder:
    Copy the snippet folder location from the Location field within the Snippets Manager. Paste this location into File Explorer.
    Copy the propdp file that comes with this code into the snippet location.
    Restart your Visual Studio.

Appending C: Creating a WPF Resource Dictionary File

You can create a ResourceDictionary file in Visual Studio via the following steps:

    Right mouse click on the folder or project within the VS Solution Explorer in which you want the ResourceDictionary to be created.
    Choose WPF folder on the left and "Resource Dictionary (WPF)" on the right:
    Choose the name of the Resource Dictionary file and press "Add" button

License

This article, along with any associated source code and files, is licensed under The Code Project Open License (CPOL)
