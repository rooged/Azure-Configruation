using Roo.Azure.Configuration.Common.Utilities.Extensions;

namespace Roo.Azure.Configuration.UnitTests
{
    public class CharExtensionsTests
    {
        private static readonly HashSet<char> letters = [ 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g',
            'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' ];

        [Test]
        [TestCase('_')]
        [TestCase('a')]
        [TestCase("")]
        [TestCase(' ')]
        [TestCase('1')]
        public void ToLower_Verify(char input)
        {
            //Act
            var result = input.ToLower();

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals('A'))
                {
                    Assert.That(result, Is.EqualTo('a'));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(input));
                }
            });
        }

        [Test]
        [TestCase('z')]
        [TestCase('M')]
        [TestCase("")]
        [TestCase(' ')]
        [TestCase('1')]
        public void ToUpper_Verify(char input)
        {
            //Act
            var result = input.ToUpper();

            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals('z'))
                {
                    Assert.That(result, Is.EqualTo('Z'));
                }
                else
                {
                    Assert.That(result, Is.EqualTo(input));
                }
            });
        }

        [Test]
        [TestCase('a')]
        [TestCase('J')]
        [TestCase("")]
        [TestCase(' ')]
        [TestCase('1')]
        public void IsLetter_Verify(char input)
        {
            //Act
            var result = input.IsLetter();

            //Assert
            Assert.Multiple(() =>
            {
                if (letters.Contains(input))
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
        [TestCase('N')]
        [TestCase('b')]
        [TestCase("")]
        [TestCase(' ')]
        [TestCase('1')]
        public void IsUpper_Verify(char input)
        {
            //Act
            var result = input.IsUpper();
            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals('N'))
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
        [TestCase('d')]
        [TestCase('L')]
        [TestCase("")]
        [TestCase(' ')]
        [TestCase('1')]
        public void IsLower_Verify(char input)
        {
            //Act
            var result = input.IsLower();
            //Assert
            Assert.Multiple(() =>
            {
                if (input.Equals('d'))
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
        [TestCase('1')]
        [TestCase('a')]
        [TestCase("")]
        [TestCase(' ')]
        [TestCase('&')]
        public void IsDigit_Verify(char input)
        {
            //Act
            var result = input.IsDigit();
            //Assert
            Assert.Multiple(() =>
            {
                if (int.TryParse(input.ToString(), out _))
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
        [TestCase('8')]
        [TestCase('p')]
        [TestCase('K')]
        [TestCase("")]
        [TestCase(' ')]
        [TestCase('&')]
        public void IsAlphanumeric_Verify(char input)
        {
            //Act
            var result = input.IsAlphanumeric();
            //Assert
            Assert.Multiple(() =>
            {
                if (int.TryParse(input.ToString(), out _) || letters.Contains(input))
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