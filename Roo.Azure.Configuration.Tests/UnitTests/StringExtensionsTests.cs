using Roo.Azure.Configuration.Common.Utilities.Extensions;

namespace Roo.Azure.Configuration.Tests.UnitTests
{
    public class StringExtensionsTests
    {
        [Test]
        [TestCase("true")]
        [TestCase("fail")]
        [TestCase("")]
        public void ToBool_Verify(string input)
        {
            //Act
            var result = input.ToBool();

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals("true"))
                {
                    Assert.That(result, Is.EqualTo(true));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(null));
                }
            });
        }

        [Test]
        [TestCase("true")]
        [TestCase("fail")]
        [TestCase("")]
        public void ToSafeBool_Verify(string input)
        {
            //Act
            var result = input.ToSafeBool();

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals("true"))
                {
                    Assert.That(result, Is.EqualTo(true));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(false));
                }
            });
        }

        [Test]
        [TestCase("5.15")]
        [TestCase("fail")]
        [TestCase("")]
        public void ToDecimal_Verify(string input)
        {
            //Act
            var result = input.ToDecimal();

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals("5.15"))
                {
                    Assert.That(result, Is.EqualTo(5.15m));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(null));
                }
            });
        }

        [Test]
        [TestCase("5.15")]
        [TestCase("fail")]
        [TestCase("")]
        public void ToSafeDecimal_Verify(string input)
        {
            //Act
            var result = input.ToSafeDecimal();

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals("5.15"))
                {
                    Assert.That(result, Is.EqualTo(5.15m));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(0m));
                }
            });
        }

        [Test]
        [TestCase("5.123456789")]
        [TestCase("fail")]
        [TestCase("")]
        public void ToDouble_Verify(string input)
        {
            //Act
            var result = input.ToDouble();

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals("5.123456789"))
                {
                    Assert.That(result, Is.EqualTo(5.123456789));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(null));
                }
            });
        }

        [Test]
        [TestCase("5.123456789")]
        [TestCase("fail")]
        [TestCase("")]
        public void ToSafeDouble_Verify(string input)
        {
            //Act
            var result = input.ToSafeDouble();

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals("5.123456789"))
                {
                    Assert.That(result, Is.EqualTo(5.123456789));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(0));
                }
            });
        }

        [Test]
        [TestCase("5")]
        [TestCase("fail")]
        [TestCase("")]
        public void ToInt_Verify(string input)
        {
            //Act
            var result = input.ToInt();

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals("5"))
                {
                    Assert.That(result, Is.EqualTo(5));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(null));
                }
            });
        }

        [Test]
        [TestCase("5")]
        [TestCase("fail")]
        [TestCase("")]
        public void ToSafeInt_Verify(string input)
        {
            //Act
            var result = input.ToSafeInt();

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals("5"))
                {
                    Assert.That(result, Is.EqualTo(5));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(0));
                }
            });
        }

        [Test]
        [TestCase("2147483648")]
        [TestCase("fail")]
        [TestCase("")]
        public void ToLong_Verify(string input)
        {
            //Act
            var result = input.ToLong();

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals("2147483648"))
                {
                    Assert.That(result, Is.EqualTo(2147483648));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(null));
                }
            });
        }

        [Test]
        [TestCase("21474836489")]
        [TestCase("fail")]
        [TestCase("")]
        public void ToSafeLong_Verify(string input)
        {
            //Act
            var result = input.ToSafeLong();

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals("21474836489"))
                {
                    Assert.That(result, Is.EqualTo(21474836489));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(0));
                }
            });
        }

        [Test]
        [TestCase("5.1234567")]
        [TestCase("fail")]
        [TestCase("")]
        public void ToFloat_Verify(string input)
        {
            //Act
            var result = input.ToFloat();

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals("5.1234567"))
                {
                    Assert.That(result, Is.EqualTo(5.1234567f));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(null));
                }
            });
        }

        [Test]
        [TestCase("5.1234567")]
        [TestCase("fail")]
        [TestCase("")]
        public void ToSafeFloat_Verify(string input)
        {
            //Act
            var result = input.ToSafeFloat();

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals("5.1234567"))
                {
                    Assert.That(result, Is.EqualTo(5.1234567f));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(0));
                }
            });
        }

        [Test]
        [TestCase("5")]
        [TestCase("fail")]
        [TestCase("")]
        public void ToShort_Verify(string input)
        {
            //Act
            var result = input.ToShort();

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals("5"))
                {
                    Assert.That(result, Is.EqualTo((short)5));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(null));
                }
            });
        }

        [Test]
        [TestCase("5")]
        [TestCase("fail")]
        [TestCase("")]
        public void ToSafeShort_Verify(string input)
        {
            //Act
            var result = input.ToSafeShort();

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals("5"))
                {
                    Assert.That(result, Is.EqualTo((short)5));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(0));
                }
            });
        }

        [Test]
        [TestCase("08/10/2025 2:30:45")]
        [TestCase("fail")]
        [TestCase("")]
        public void ToDateTime_Verify(string input)
        {
            //Act
            var result = input.ToDateTime();

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals("08/10/2025 2:30:45"))
                {
                    Assert.That(result, Is.EqualTo(new DateTime(2025, 8, 10, 2, 30, 45)));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(null));
                }
            });
        }

        [Test]
        [TestCase("08/10/2025 2:30:45")]
        [TestCase("fail")]
        [TestCase("")]
        public void ToSafeDateTime_Verify(string input)
        {
            //Act
            var result = input.ToSafeDateTime();

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals("08/10/2025 2:30:45"))
                {
                    Assert.That(result, Is.EqualTo(new DateTime(2025, 8, 10, 2, 30, 45)));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(DateTime.MinValue));
                }
            });
        }

        [Test]
        [TestCase("27fc633b-8b18-44fa-90e0-2b0be9de284b")]
        [TestCase("fail")]
        [TestCase("")]
        public void ToGuid_Verify(string input)
        {
            //Act
            var result = input.ToGuid();

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals("27fc633b-8b18-44fa-90e0-2b0be9de284b"))
                {
                    Assert.That(result, Is.EqualTo(Guid.Parse("27fc633b-8b18-44fa-90e0-2b0be9de284b")));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(null));
                }
            });
        }

        [Test]
        [TestCase("27fc633b-8b18-44fa-90e0-2b0be9de284b")]
        [TestCase("fail")]
        [TestCase("")]
        public void ToSafeGuid_Verify(string input)
        {
            //Act
            var result = input.ToSafeGuid();

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals("27fc633b-8b18-44fa-90e0-2b0be9de284b"))
                {
                    Assert.That(result, Is.EqualTo(Guid.Parse("27fc633b-8b18-44fa-90e0-2b0be9de284b")));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(Guid.Empty));
                }
            });
        }

        [Test]
        [TestCase("a test string, ABC, aNd aPPLEs", true)]
        [TestCase("a test string, ABC, aNd aPPLEs", false)]
        [TestCase("", true)]
        public void ToTitleCase_Verify(string input, bool convertAllCapitals)
        {
            //Act
            var result = input.ToTitleCase(convertAllCapitals);

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals("a test string, ABC, aNd aPPLEs"))
                {
                    if (convertAllCapitals)
                    {
                        Assert.That(result, Is.EqualTo("A Test String, Abc, And Apples"));
                    }
                    else
                    {
                        Assert.That(result, Is.EqualTo("A Test String, ABC, And Apples"));
                    }
                }
                else
                {
                    Assert.That(result, Is.EqualTo(string.Empty));
                }
            });
        }

        [Test]
        [TestCase("123-456-7890")]
        [TestCase("$1,234,567,890")]
        [TestCase("fail")]
        [TestCase("")]
        public void ToNumbersOnly_Verify(string input)
        {
            //Act
            var result = input.ToNumbersOnly();

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals("123-456-7890"))
                {
                    Assert.That(result, Is.EqualTo("1234567890"));
                }
                else if (input.Equals("$1,234,567,890"))
                {
                    Assert.That(result, Is.EqualTo("1234567890"));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(string.Empty));
                }
            });
        }

        [Test]
        [TestCase("passfail")]
        [TestCase("pas")]
        [TestCase("")]
        public void Truncate_Verify(string input)
        {
            //Act
            var result = input.Truncate(4);

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals("passfail"))
                {
                    Assert.That(result, Is.EqualTo("pass"));
                }
                else if (input.Equals("pas"))
                {
                    Assert.That(result, Is.EqualTo("pas"));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(string.Empty));
                }
            });
        }

        [Test]
        [TestCase("pass@email.com")]
        [TestCase("failemail.com")]
        [TestCase("")]
        public void IsValidEmailFormat_Verify(string input)
        {
            //Act
            var result = input.IsValidEmailFormat();

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals("pass@email.com"))
                {
                    Assert.That(result, Is.EqualTo(true));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(false));
                }
            });
        }

        [Test]
        [TestCase("abc123!@#$%^&'\"def")]
        [TestCase("")]
        public void ToSafeString_Verify(string input)
        {
            //Act
            var result = input.ToSafeString();

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals("abc123!@#$%^&'\"def"))
                {
                    Assert.That(result, Is.EqualTo("abc123!@#$%^&amp;&#39;&quot;def"));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(string.Empty));
                }
            });
        }

        [Test]
        [TestCase("test")]
        [TestCase("testpass")]
        [TestCase("")]
        public void EnsureEndsWith_Verify(string input)
        {
            //Act
            var result = input.EnsureEndsWith("pass");

            //Assert
            Assert.Multiple(() =>
            {
                if (!input.Equals(""))
                {
                    Assert.That(result, Is.EqualTo("testpass"));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(string.Empty));
                }
            });
        }

        [Test]
        [TestCase("test@email.com")]
        [TestCase("test123pass@email.com")]
        [TestCase("")]
        public void MaskEmail_Verify(string input)
        {
            //Act
            var result = input.MaskEmail();

            //Assert
            Assert.Multiple(() =>
            {
                if (!input.Equals(""))
                {
                    Assert.That(result, Is.EqualTo("te***@email.com"));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(string.Empty));
                }
            });
        }

        [Test]
        [TestCase("pass")]
        [TestCase("Pass")]
        [TestCase("")]
        public void FirstCharToUpper_Verify(string input)
        {
            //Act
            var result = input.FirstCharToUpper();

            //Assert
            Assert.Multiple(() =>
            {
                if (!input.Equals(""))
                {
                    Assert.That(result, Is.EqualTo("Pass"));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(string.Empty));
                }
            });
        }

        [Test]
        [TestCase("pass")]
        [TestCase("")]
        public void Base64Encode_Verify(string input)
        {
            //Act
            var result = input.Base64Encode();

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals("pass"))
                {
                    Assert.That(result, Is.Not.EqualTo("pass"));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(string.Empty));
                }
            });
        }

        [Test]
        [TestCase("pass")]
        [TestCase("")]
        [TestCase(null)]
        public void IsNullOrEmpty_Verify(string? input)
        {
            //Act
            var result = input.IsNullOrEmpty();

            //Assert
            Assert.Multiple(() =>
            {
                if (input == null || input.Equals(""))
                {
                    Assert.That(result, Is.EqualTo(true));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(false));
                }
            });
        }
    }
}