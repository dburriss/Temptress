using System;

#if NET46 || NET452 || NET451 || DNX46 || DNX452 || DNX451
using System.ComponentModel;
#endif

namespace Temptress.TestMessages
{
    public abstract class EmailMessage
    {
    }

    public class WelcomeMessage :EmailMessage
    {
        public string FullName { get; set; }

        public DateTime JoinDate { get; set; }

        public Address Address { get; set; }
    }

    public class OtherWelcomeMessage
    {
#if NET46 || NET452 || NET451 || DNX46 || DNX452 || DNX451
        [DisplayName("Full Name")]
#endif
        public string FullName { get; set; }

#if NET46 || NET452 || NET451 || DNX46 || DNX452 || DNX451
        [DisplayName("Join Date")]
#endif
        public DateTime JoinDate { get; set; }

        public Address Address { get; set; }
    }
 
    public class Address
    {
#if NET46 || NET452 || NET451 || DNX46 || DNX452 || DNX451
        [DisplayName("Street Number")]
#endif
        public int StreetNumber { get; set; }
#if NET46 || NET452 || NET451 || DNX46 || DNX452 || DNX451
        [DisplayName("Street Name")]
#endif
        public string StreetName { get; set; }
    }
}
