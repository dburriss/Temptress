# Temptress
I am a handle bars like templating engine for .NET.

Temptress uses the `{{var}}` type syntax for replacing text with values.
A big push for this project is that messages are strongly typed and verifiable*(TODO)* against the template.

> Requires the reflection helper library [Philosphical Monkey](https://github.com/dburriss/PhilosophicalMonkey).

## Basic Usage

With the following POCO classes defined:

    public class WelcomeMessage
    {
        public string FullName { get; set; }

        public DateTime JoinDate { get; set; }

        public Address Address { get; set; }
    }
    
    public class Address
    {
        public int StreetNumber { get; set; }
        public string StreetName { get; set; }
    }
 
 Then creating a template:   
 
    var template = new Template<WelcomeMessage>("Hello {{FullName}} from {{Address.StreetNumber}} {{Address.StreetName}}!");
    var renderer = new TemplateRenderer<WelcomeMessage>(template);
    
Then you can render the template with whatever data you like, reusing the same template with different data.

> Note the creation template is the expensive part (as much as possible) so that the rendering is as fast as possible.

Then rendering the content:

    var message = new WelcomeMessage(){ FullName = "Devon", Address = new Address{ StreetNumber = 5, StreetName = "Main"} };
    var messageText = renderer.Render(message);
    