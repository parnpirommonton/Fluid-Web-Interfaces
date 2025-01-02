# Fluid Web Interfaces

A HTML parser that allows for custom syntax to be used within HTML and a Website compiler that compiles directories using this custom HTML syntax into plain HTML in the output directory. A desktop app is also included which helps with interaction with the compiler. 

## The HTML Parser (DynamicWebClientParser)

This is the parser that reads the source HTML file with the custom syntax and converts it into a class structure so that the compiler has an easier time creating the output HTML file.

### Current Features

#### JavaScript Variable Binding

This allows the user of this framework to have JavaScript variable values to be rendered on webpages at will. The variable content is rendered whenever the user runs the 'onStateChanged()' function.
The user can type wherever they would normally type out text within a HTML document (e.g. attribute values, or innerHTML) the name of the JavaScript variable surrounded by curly braces.

JavaScript:
let WebpageTitle = "This is my Webpage!";

HTML:
'''
<p>{WebpageTitle}</p>
'''

The HTML would then render 'This is my Webpage!' on the webpage. If the user were to then change the value of the 'WebpageTitle' variable and then run the 'onStateChanged()' function, the value on the webpage would then update to reflect the current value.

Although the binding syntax was intended for JavaScript variables It does also support execution of JavaScript functions.
The following syntax is valid.

HTML:
'''
<p>{new Date()}</p>
<p>{someFunction()}</p>
'''

#### Render Command

This command is used to inject partial HTML files (which are marked by beginning a HTML file name with an underscore '_') into the current HTML file.

The syntax for the command is as follows:

HTML:
'''
@render "./_navigation.htm"
'''

This would add the contents of the '_navigation.htm' file into the current one where the render command is located.

#### If Statements

If statements evaluate JavaScript conditions and render HTML code depending on if the condition returns true.
If statements can be embedded into your webpage using the following syntax:

HTML:
'''
@If (Quality > 0.75)
{
    <p>Excellent Work!</p>
}
elseif (Quality > 0.4)
{
    <p>Good Work.</p>
}
else
{
    <p>Horrible.</p>
}
'''

Again, these the evaluation can be refreshed by using the 'onStateChanged()' function.

### Planned Features

- Foreach Statements
- For Statements
- Import Command





