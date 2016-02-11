using System;
using System.Collections.Generic;
using System.Text;
using Temptress.TestMessages;
using Xunit;

namespace Temptress.UnitTests
{
    public class TemptressContentTests
    {
        [Fact]
        public void Render_WithNullProperty_ReturnsEmptyString()
        {
            var message = new WelcomeMessage(){ FullName = null };
            var template = new Template<WelcomeMessage>("Hello {{FullName}}!");
            var content = new TemplateRenderer<WelcomeMessage>(template);
            var text = content.Render(message);

            Assert.Contains("Hello !", text);
        }

        [Fact]
        public void Render_WithX_ReturnsString()
        {
            var message = new WelcomeMessage(){ FullName = "Devon" };
            var template = new Template<WelcomeMessage>("Hello {{FullName}}!");
            var content = new TemplateRenderer<WelcomeMessage>(template);
            var text = content.Render(message);

            Assert.False(string.IsNullOrEmpty(text));
        }

        [Fact]
        public void Render_WithHelloWorldTemplate_ReturnsHelloDevon()
        {
            var message = new WelcomeMessage(){ FullName = "Devon" };
            var template = new Template<WelcomeMessage>("Hello {{FullName}}!");
            var content = new TemplateRenderer<WelcomeMessage>(template);
            var text = content.Render(message);

            Assert.Equal("Hello Devon!", text);
        }

        [Fact]
        public void Render_WithTemplateForNullable_ReturnsValueIs5()
        {
            var message = new ComplexModel(){ NullIntProp = 5 };
            var template = new Template<ComplexModel>("Value is {{NullIntProp}}");
            var content = new TemplateRenderer<ComplexModel>(template);
            var text = content.Render(message);

            Assert.Equal("Value is 5", text);
        }

        [Fact]
        public void Render_TemplateWithNavigationalProperties_TransformsNestedProperties()
        {
            var message = new WelcomeMessage(){ FullName = "Devon", Address = new Address{ StreetName = "Main"} };
            var template = new Template<WelcomeMessage>("Hello {{FullName}} from {{Address.StreetName}}!");
            var content = new TemplateRenderer<WelcomeMessage>(template);
            var text = content.Render(message);

            Assert.Contains("Main", text);
        }

        [Fact]
        public void Render_TemplateWithInteger_TransformsNestedProperties()
        {
            var message = new WelcomeMessage(){ FullName = "Devon", Address = new Address{ StreetNumber = 5, StreetName = "Main"} };
            var template = new Template<WelcomeMessage>("Hello {{FullName}} from {{Address.StreetNumber}} {{Address.StreetName}}!");
            var content = new TemplateRenderer<WelcomeMessage>(template);
            var text = content.Render(message);

            Assert.Contains("5", text);
        }

        [Fact]
        public void Render_TemplateWithDate_TransformsNestedProperties()
        {
            string myFormat = "yyyy-MM-dd";
            DateTime date = DateTime.Parse("2014-08-30");
            var message = new WelcomeMessage(){ FullName = "Devon", JoinDate = date, Address = new Address{ StreetNumber = 5, StreetName = "Main"} };
            var template = new Template<WelcomeMessage>("Hello {{FullName}} from {{Address.StreetNumber}} {{Address.StreetName}}! on {{JoinDate}}");
            var content = new TemplateRenderer<WelcomeMessage>(template)
            {
                RenderSettings = new RenderSettings
                {
                    DateTimeFormat = myFormat
                }
            };
            var text = content.Render(message);

            Assert.Contains("Hello Devon from 5 Main! on ", text);
            Assert.Contains("on 2014-08-30", text);
        }

        [Fact]
        public void Render_TemplateWithIEnumerable_RenderIEnumerableOfString()
        {
            var message = new ComplexModel
            {
                BunchOfStrings = new List<string>() { "ONE", "TWO"}
            };

            var template = new Template<ComplexModel>("{{BunchOfStrings}}");
            var content = new TemplateRenderer<ComplexModel>(template);
            var text = content.Render(message);

            Assert.Contains("ONE, TWO", text);
        }

        [Fact(Skip = "TO IMPLEMENT")]
        public void Render_TemplateWithForeach_RenderIEnumerableOfString()
        {
            var message = new ComplexModel
            {
                BunchOfStrings = new List<string>() { "ONE", "TWO"}
            };

            string templateText = @"<<foreach:BunchOfStrings>>{{value}} <</foreach:BunchOfStrings>>";
            var template = new Template<ComplexModel>(templateText);
            var content = new TemplateRenderer<ComplexModel>(template);
            var text = content.Render(message);

            Assert.Contains("ONE", text);
            Assert.Contains("TWO", text);
        }

        [Fact(Skip = "TO IMPLEMENT")]
        public void Render_TemplateWithForeach_RenderIEnumerableOfAddresses()
        {
            //for this potentially can extract whole section and then render it and re-insert
            var message = new ComplexModel
            {
                SomeText = "Bob",
                BunchOfAddresses = new List<Address>() {
                new Address{ StreetNumber = 5, StreetName = "Main"},
                new Address{ StreetNumber = 10, StreetName = "Main"}
                }
            };

            string templateText = @"{{SomeText}} <<foreach:BunchOfAddresses>>{{StreetNumber}} {{StreetName}} <</foreach:BunchOfAddresses>>";
            var template = new Template<ComplexModel>(templateText);
            var content = new TemplateRenderer<ComplexModel>(template);
            var text = content.Render(message);

            Assert.Contains("5 Main", text);
            Assert.Contains("10 Main", text);
            Assert.Contains("Bob", text);
        }

        [Fact(Skip="TO IMPLEMENT")]
        public void Render_WithControlSectionForNestedroperty_ReturnsNoItems()
        {
            string templateText = @"<<foreach:SomeProperty.BunchOfAddresses>>{{StreetNumber}} {{StreetName}} <</foreach:SomeProperty.BunchOfAddresses>>";
            var template = new Template<ComplexModel>(templateText);

           
        }

        [Fact]
        public void Render_UsingNonGeneric_ReplacesValues()
        {
            string templateText = "Hello {{FullName}} from {{Address.StreetNumber}} {{Address.StreetName}}! on {{JoinDate}}";
            ITemplate template = BuildTemplate(1, templateText);

            var templateContent = new TemplateContent(template);
            var message = new WelcomeMessage(){ FullName = "Devon", Address = new Address{ StreetName = "Main"} };

            var text = templateContent.Render(message);

            Assert.Contains("Devon", text);
        }

        [Fact]
        public void Ctor_WithInterface_CreatesInstance()
        {
            string templateText = "Hello {{FullName}} from {{Address.StreetNumber}} {{Address.StreetName}}! on {{JoinDate}}";
            ITemplate template = BuildTemplate(1, templateText);

            var content = new TemplateContent(template);

            Assert.NotNull(content);
        }

        [Fact]
        public void Render_FullName10000Times_InUnderHalfSecond()
        {
            int occurances = 10000;
            
            string templateText = "{{FullName}}";
            var template = BuildTemplate(occurances, templateText);
            var message = new WelcomeMessage() { FullName = "Devon" };
            
            var content = new TemplateRenderer<WelcomeMessage>(template);
            var before = DateTime.Now.Ticks;

            var text = content.Render(message);

            var after = DateTime.Now.Ticks;
            var elapsedSpan = new TimeSpan(after - before);

            Assert.True(elapsedSpan.TotalSeconds < 0.5);
        }

        [Fact]
        public void Render_FourMergeOptions2500Times_InUnderHalfSecond()
        {
            int occurances = 2500;

            string templateText = "Hello {{FullName}} from {{Address.StreetNumber}} {{Address.StreetName}}! on {{JoinDate}}";
            var template = BuildTemplate(occurances, templateText);

            DateTime date = DateTime.Parse("2014-08-30");
            var message = new WelcomeMessage(){ FullName = "Devon", JoinDate = date, Address = new Address{ StreetNumber = 5, StreetName = "Main"} };
            
            var content = new TemplateRenderer<WelcomeMessage>(template);
            
            var before = DateTime.Now.Ticks;

            var text = content.Render(message);

            var after = DateTime.Now.Ticks;
            var elapsedSpan = new TimeSpan(after - before);

            Assert.True(elapsedSpan.TotalSeconds < 0.5);
        }

        [Fact]
        public void Render_FourMergeOptions2500TimesTwice_InUnderOneSecond()
        {
            int occurances = 2500;

            string templateText = "Hello {{FullName}} from {{Address.StreetNumber}} {{Address.StreetName}}! on {{JoinDate}}";
            var template = BuildTemplate(occurances, templateText);

            DateTime date = DateTime.Parse("2014-08-30");
            var message = new WelcomeMessage(){ FullName = "Devon", JoinDate = date, Address = new Address{ StreetNumber = 5, StreetName = "Main"} };
            
            var content = new TemplateRenderer<WelcomeMessage>(template);
            
            var before = DateTime.Now.Ticks;

            var text1 = content.Render(message);
            var text2 = content.Render(message);

            var after = DateTime.Now.Ticks;
            var elapsedSpan = new TimeSpan(after - before);

            Assert.True(elapsedSpan.TotalSeconds < 1);
        }
 
        private Template<WelcomeMessage> BuildTemplate(int occurances, string templateText)
        {
            StringBuilder templateBuilder = new StringBuilder();
            for (int i = 0; i < occurances; i++)
            {
                templateBuilder.AppendLine(templateText);
            }
            string templateString = templateBuilder.ToString();
            var template = new Template<WelcomeMessage>(templateString);
            return template;
        }

    }
}
