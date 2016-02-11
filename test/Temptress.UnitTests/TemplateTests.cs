using System.Collections.Generic;
using Temptress.TestMessages;
using Xunit;
using System.Linq;

namespace Temptress.UnitTests
{
    public class TemplateTests
    {
        [Fact]
        public void TemplateMergeOptions_WithPropertiesOnMessage_ReturnsOptions()
        {
            var templateText = "";
            var template = new Template<WelcomeMessage>(templateText);

            var possibleMergeOptions = template.MergeValues();

            Assert.NotEmpty(possibleMergeOptions);
        }

        [Fact]
        public void TemplateMergeValueNames_WithFullNameOnMessage_ReturnsFullNameInList()
        {
            var templateText = "";
            var template = new Template<WelcomeMessage>(templateText);

            var possibleMergeOptions = template.MergeNames();

            Assert.True(possibleMergeOptions.Any(x => x.Equals("FullName")));
        }

        [Fact]
        public void TemplateMergeValueNames_WithNullableOnClass_ReturnsReadableValue()
        {
            var templateText = "";
            var template = new Template<ComplexModel>(templateText);

            var possibleMergoOptions = template.MergeNames();

            Assert.True(possibleMergoOptions.Any(x => x.Equals("NullIntProp")));
        }

        [Fact]
        public void TemplateMergeValueNames_WithAddressOnMessage_ReturnsAddressStreetName()
        {
            var templateText = "";
            var template = new Template<WelcomeMessage>(templateText);

            var possibleMergoOptions = template.MergeNames();

            Assert.True(possibleMergoOptions.Any(x => x.Equals("Address.StreetName")));
        }

        [Fact]
        public void TemplateMergeValueNames_WithAddressAndPlaceOnMessage_DoesNotMergeNavigationalPath()
        {
            var templateText = "";
            var template = new Template<ComplexModel>(templateText);

            var possibleMergoOptions = template.MergeNames();

            Assert.False(possibleMergoOptions.Any(x => x.Equals("Address.Place.StreetName")));
            Assert.False(possibleMergoOptions.Any(x => x.Equals("Place.Address.StreetName")));
            Assert.False(possibleMergoOptions.Any(x => x.Equals("Address.Address.StreetName")));
        }

        [Fact]
        public void TemplateMergeOptions_WithFullNameOnMessage_ReturnsFullNameWithBrackets()
        {
            var templateText = "";
            var template = new Template<WelcomeMessage>(templateText);

            var possibleMergoOptions = template.MergeValues();

            Assert.True(possibleMergoOptions.Any(x => x.Equals("{{FullName}}")));
        }

        [Fact]
        public void TemplateMergeOptions_WithAddressStreetNameOnMessage_ReturnsAddressStreetNameWithBrackets()
        {
            var templateText = "";
            var template = new Template<WelcomeMessage>(templateText);

            var possibleMergoOptions = template.MergeValues();

            Assert.True(possibleMergoOptions.Any(x => x.Equals("{{Address.StreetName}}")));
        }

        [Fact]
        public void FriendlyTemplateTypeMergeOptions_WithAttribute_ReturnsDisplay()
        {
            var templateText = "";
            var template = new Template<OtherWelcomeMessage>(templateText);

            var possibleMergoOptions = template.FriendlyMergeLabels();

            Assert.True(possibleMergoOptions.Any(x => x.Equals("Join Date")));
            Assert.True(possibleMergoOptions.Any(x => x.Equals("Street Name")));
        }

        [Fact]
        public void IsValid_WithMatchingProperty_ReturnsTrue()
        {
            var templateText = "{{FullName}}";
            var template = new Template<WelcomeMessage>(templateText);

            Assert.True(template.IsValid());
        }

        [Fact]
        public void IsValid_WithMatchingNestedProperty_ReturnsTrue()
        {
            var templateText = "{{Address.StreetName}}";
            var template = new Template<WelcomeMessage>(templateText);

            Assert.True(template.IsValid());
        }

        [Fact]
        public void IsValid_WithoutMatchingProperty_ReturnsFalse()
        {
            var templateText = "{{X}}";
            var template = new Template<WelcomeMessage>(templateText);

            Assert.False(template.IsValid());
        }

        [Fact]
        public void IsValid_WithoutMatchingNestedProperty_ReturnsFalse()
        {
            var templateText = "{{Address.X}}";
            var template = new Template<WelcomeMessage>(templateText);

            Assert.False(template.IsValid());
        }

        [Fact]
        public void HasControlSections_WithoutControlSection_ReturnsFalse()
        {
            var templateText = "";
            var template = new Template<WelcomeMessage>(templateText);

            Assert.False(template.HasControlSections);
        }

        [Fact]
        public void HasControlSections_WithControlSection_ReturnsTrue()
        {
            string templateText = @"<<foreach:BunchOfStrings>>{{value}} <</foreach:BunchOfStrings>>";
            var template = new Template<ComplexModel>(templateText);

            Assert.True(template.HasControlSections);
        }

        [Fact]
        public void ControlSections_With1ControlSection_Returns1Items()
        {
            string templateText = @"<<foreach:BunchOfAddresses>>{{StreetNumber}} {{StreetName}} <</foreach:BunchOfAddresses>>";
            
            var template = new Template<ComplexModel>(templateText);

            Assert.True(template.ControlSections.Count() == 1);
        }

        [Fact(Skip = "TO IMPLEMENT")]
        public void ControlSections_With2ControlSection_Returns2Items()
        {
            string templateText = @"<<foreach:BunchOfAddresses>>1: {{StreetNumber}} {{StreetName}} <</foreach>><<foreach:BunchOfAddresses>>2: {{StreetNumber}} {{StreetName}} <</foreach:BunchOfAddresses>>";
            var template = new Template<ComplexModel>(templateText);

            Assert.True(template.ControlSections.Count() == 2);
        }

    }
} 