using Roo.Azure.Configuration.Common.Mapper;
using Roo.Azure.Configuration.Common.Utilities.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Roo.Azure.Configuration.UnitTests
{
    public class RooMapperTests
    {
        [Test]
        public void AutoMap_Verify()
        {
            //Arrange
            var mappingConfig = new RooMapperManager(x =>
            {
                x.AddProfile(new AutoReverseMap());
            });
            var mapper = mappingConfig.CreateMapper();
            var source = new Source();
            var destination = new Destination();

            //Act
            destination = mapper.Map<Destination>(source);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(destination?.String, Is.EqualTo(source.String));
                Assert.That(destination?.StringNull, Is.Null);
                Assert.That(destination?.Int, Is.EqualTo(source.Int));
                Assert.That(destination?.IntNull, Is.Null);
                Assert.That(destination?.Double, Is.EqualTo(source.Double));
                Assert.That(destination?.DoubleNull, Is.Null);
                Assert.That(destination?.Decimal, Is.EqualTo(source.Decimal));
                Assert.That(destination?.DecimalNull, Is.Null);
                Assert.That(destination?.Bool, Is.EqualTo(source.Bool));
                Assert.That(destination?.BoolNull, Is.Null);
                Assert.That(destination?.Byte, Is.EqualTo(source.Byte));
                Assert.That(destination?.ByteNull, Is.Null);
                Assert.That(destination?.DateTime, Is.EqualTo(source.DateTime));
                Assert.That(destination?.DateTimeNull, Is.Null);
            });
        }

        [Test]
        public void ReverseMap_Verify()
        {
            //Arrange
            var mappingConfig = new RooMapperManager(x =>
            {
                x.AddProfile(new AutoReverseMap());
            });
            var mapper = mappingConfig.CreateMapper();
            var source = new Source();
            var destination = new Destination();

            //Act
            source = mapper.Map<Source>(destination);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(source?.String, Is.EqualTo(destination.String));
                Assert.That(source?.StringNull, Is.Null);
                Assert.That(source?.Int, Is.EqualTo(destination.Int));
                Assert.That(source?.IntNull, Is.Null);
                Assert.That(source?.Double, Is.EqualTo(destination.Double));
                Assert.That(source?.DoubleNull, Is.Null);
                Assert.That(source?.Decimal, Is.EqualTo(destination.Decimal));
                Assert.That(source?.DecimalNull, Is.Null);
                Assert.That(source?.Bool, Is.EqualTo(destination.Bool));
                Assert.That(source?.BoolNull, Is.Null);
                Assert.That(source?.Byte, Is.EqualTo(destination.Byte));
                Assert.That(source?.ByteNull, Is.Null);
                Assert.That(source?.DateTime, Is.EqualTo(destination.DateTime));
                Assert.That(source?.DateTimeNull, Is.Null);
            });
        }

        [Test]
        public void CustomMap_Verify()
        {
            //Arrange
            var mappingConfig = new RooMapperManager(x =>
            {
                x.AddProfile(new CustomMap());
            });
            var mapper = mappingConfig.CreateMapper();
            var source = new Source();
            var destination = new Destination();

            //Act
            destination = mapper.Map<Destination>(source);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(destination?.String, Is.EqualTo(source.String));
                Assert.That(destination?.StringNull, Is.EqualTo(source.String));
                Assert.That(destination?.Int, Is.EqualTo(source.Int));
                Assert.That(destination?.IntNull, Is.EqualTo(source.Int));
                Assert.That(destination?.Double, Is.EqualTo(source.Double));
                Assert.That(destination?.DoubleNull, Is.EqualTo(source.Double));
                Assert.That(destination?.Decimal, Is.EqualTo(source.Decimal));
                Assert.That(destination?.DecimalNull, Is.EqualTo(source.Decimal));
                Assert.That(destination?.Bool, Is.EqualTo(source.Bool));
                Assert.That(destination?.BoolNull, Is.EqualTo(source.Bool));
                Assert.That(destination?.Byte, Is.EqualTo(source.Byte));
                Assert.That(destination?.ByteNull, Is.EqualTo(source.Byte));
                Assert.That(destination?.DateTime, Is.EqualTo(source.DateTime));
                Assert.That(destination?.DateTimeNull, Is.EqualTo(source.DateTime));
            });
        }

        [Test]
        public void CustomReverseMap_Verify()
        {
            //Arrange
            var mappingConfig = new RooMapperManager(x =>
            {
                x.AddProfile(new CustomMap());
            });
            var mapper = mappingConfig.CreateMapper();
            var source = new Source();
            var destination = new Destination();

            //Act
            source = mapper.Map<Source>(destination);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(source?.String, Is.EqualTo(destination.String));
                Assert.That(source?.Int, Is.EqualTo(destination.Int));
                Assert.That(source?.Double, Is.EqualTo(destination.Double));
                Assert.That(source?.Decimal, Is.EqualTo(destination.Decimal));
                Assert.That(source?.Bool, Is.EqualTo(destination.Bool));
                Assert.That(source?.Byte, Is.EqualTo(destination.Byte));
                Assert.That(source?.DateTime, Is.EqualTo(destination.DateTime));
            });
        }

        [Test]
        public void IgnoreMap_Verify()
        {
            //Arrange
            var mappingConfig = new RooMapperManager(x =>
            {
                x.AddProfile(new IgnoreMap());
            });
            var mapper = mappingConfig.CreateMapper();
            var source = new Source();
            var destination = new Destination();

            //Act
            destination = mapper.Map<Destination>(source);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(destination?.String, Is.EqualTo("test2"));
                Assert.That(destination?.StringNull, Is.EqualTo(source.String));
                Assert.That(destination?.Int, Is.EqualTo(2));
                Assert.That(destination?.IntNull, Is.EqualTo(source.Int));
                Assert.That(destination?.Double, Is.EqualTo(2));
                Assert.That(destination?.DoubleNull, Is.EqualTo(source.Double));
                Assert.That(destination?.Decimal, Is.EqualTo(2m));
                Assert.That(destination?.DecimalNull, Is.EqualTo(source.Decimal));
                Assert.That(destination?.Bool, Is.EqualTo(false));
                Assert.That(destination?.BoolNull, Is.EqualTo(source.Bool));
                Assert.That(destination?.Byte, Is.EqualTo(2));
                Assert.That(destination?.ByteNull, Is.EqualTo(source.Byte));
                Assert.That(destination?.DateTime, Is.EqualTo(DateTime.Now.Date.AddDays(1)));
                Assert.That(destination?.DateTimeNull, Is.EqualTo(source.DateTime));
            });
        }

        [Test]
        public void ConstantMap_Verify()
        {
            //Arrange
            var mappingConfig = new RooMapperManager(x =>
            {
                x.AddProfile(new ConstantMap());
            });
            var mapper = mappingConfig.CreateMapper();
            var source = new Source();
            var destination = new Destination();

            //Act
            destination = mapper.Map<Destination>(source);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(destination?.String, Is.EqualTo("testing"));
                Assert.That(destination?.StringNull, Is.EqualTo(DateTime.Now.Date.ToString()));
                Assert.That(destination?.Int, Is.EqualTo(3));
                Assert.That(destination?.IntNull, Is.EqualTo(3));
                Assert.That(destination?.Double, Is.EqualTo(1.1));
                Assert.That(destination?.DoubleNull, Is.EqualTo(1.1));
                Assert.That(destination?.Decimal, Is.EqualTo(1.1m));
                Assert.That(destination?.DecimalNull, Is.EqualTo(1.1m));
                Assert.That(destination?.Bool, Is.EqualTo(true));
                Assert.That(destination?.BoolNull, Is.EqualTo(true));
                Assert.That(destination?.Byte, Is.EqualTo(3));
                Assert.That(destination?.ByteNull, Is.EqualTo(3));
                Assert.That(destination?.DateTime, Is.EqualTo(DateTime.Now.Date.AddDays(-2)));
                Assert.That(destination?.DateTimeNull, Is.EqualTo(DateTime.Now.Date));
            });
        }

        [Test]
        public void ComputedMap_Verify()
        {
            //Arrange
            var mappingConfig = new RooMapperManager(x =>
            {
                x.AddProfile(new ComputedMap());
            });
            var mapper = mappingConfig.CreateMapper();
            var source = new Source();
            var destination = new Destination();

            //Act
            destination = mapper.Map<Destination>(source);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(destination?.String, Is.EqualTo(source.String + source.String));
                Assert.That(destination?.StringNull, Is.EqualTo($"{source.String} testing"));
                Assert.That(destination?.Int, Is.EqualTo(source.Int + source.Int + source.Int));
                Assert.That(destination?.IntNull, Is.EqualTo((int)source.Double));
                Assert.That(destination?.Double, Is.EqualTo(source.Double * 2));
                Assert.That(destination?.DoubleNull, Is.EqualTo(source.Double + 0.1));
                Assert.That(destination?.Decimal, Is.EqualTo(source.Decimal + source.Decimal));
                Assert.That(destination?.DecimalNull, Is.EqualTo(0.5m));
                Assert.That(destination?.Bool, Is.EqualTo(true));
                Assert.That(destination?.BoolNull, Is.EqualTo(true));
                Assert.That(destination?.Byte, Is.EqualTo(0));
                Assert.That(destination?.ByteNull, Is.EqualTo(1));
                Assert.That(destination?.DateTime, Is.EqualTo(DateTime.Now.Date.AddDays(-1)));
                Assert.That(destination?.DateTimeNull, Is.EqualTo(DateTime.Now.Date));
            });
        }

        [Test]
        public void ListMap_Verify()
        {
            //Arrange
            var mappingConfig = new RooMapperManager(x =>
            {
                x.AddProfile(new ListMap());
            });
            var mapper = mappingConfig.CreateMapper();
            var source = new List<Source>() { new(), new() { String = "test3", Int = 3, DateTime = DateTime.Now.Date.AddDays(5) } };
            var destination = new List<Destination>();

            //Act
            destination = mapper.Map<List<Destination>>(source);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(destination?[0].String, Is.EqualTo(source[0].String));
                Assert.That(destination?[0].StringNull, Is.EqualTo(source[0].String));
                Assert.That(destination?[0].Int, Is.EqualTo(source[0].Int));
                Assert.That(destination?[0].IntNull, Is.EqualTo(source[0].Int));
                Assert.That(destination?[0].Double, Is.EqualTo(2));
                Assert.That(destination?[0].DoubleNull, Is.Null);
                Assert.That(destination?[0].Decimal, Is.EqualTo(source[0].Decimal));
                Assert.That(destination?[0].DecimalNull, Is.EqualTo(source[0].Decimal));
                Assert.That(destination?[0].Bool, Is.EqualTo(source[0].Bool));
                Assert.That(destination?[0].BoolNull, Is.EqualTo(source[0].Bool));
                Assert.That(destination?[0].Byte, Is.EqualTo(source[0].Byte));
                Assert.That(destination?[0].ByteNull, Is.EqualTo(source[0].Byte));
                Assert.That(destination?[0].DateTime, Is.EqualTo(source[0].DateTime));
                Assert.That(destination?[0].DateTimeNull, Is.EqualTo(source[0].DateTime));
                Assert.That(destination?[1].String, Is.EqualTo(source[1].String));
                Assert.That(destination?[1].StringNull, Is.EqualTo(source[1].String));
                Assert.That(destination?[1].Int, Is.EqualTo(source[1].Int));
                Assert.That(destination?[1].IntNull, Is.EqualTo(source[1].Int));
                Assert.That(destination?[1].DateTime, Is.EqualTo(source[1].DateTime));
                Assert.That(destination?[1].DateTimeNull, Is.EqualTo(source[1].DateTime));
            });
        }

        [Test]
        public void MultipleProfilesMap_Verify()
        {
            //Arrange
            var mappingConfig = new RooMapperManager(x =>
            {
                x.AddProfile(new MultipleProfilesMap());
            });
            var mapper = mappingConfig.CreateMapper();
            var source = new Source();
            var destination = new Destination();
            var source2 = new Source2();
            var destination2 = new Destination2();

            //Act
            destination = mapper.Map<Destination>(source);
            destination2 = mapper.Map<Destination2>(source2);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(destination?.String, Is.EqualTo(source.String));
                Assert.That(destination?.StringNull, Is.EqualTo(source.String));
                Assert.That(destination?.Int, Is.EqualTo(source.Int + 2));
                Assert.That(destination?.IntNull, Is.Null);
                Assert.That(destination?.Double, Is.EqualTo(source.Double));
                Assert.That(destination?.DoubleNull, Is.Null);
                Assert.That(destination?.Decimal, Is.EqualTo(source.Decimal));
                Assert.That(destination?.DecimalNull, Is.Null);
                Assert.That(destination?.Bool, Is.EqualTo(source.Bool));
                Assert.That(destination?.BoolNull, Is.Null);
                Assert.That(destination?.Byte, Is.EqualTo(source.Byte));
                Assert.That(destination?.ByteNull, Is.Null);
                Assert.That(destination?.DateTime, Is.EqualTo(source.DateTime));
                Assert.That(destination?.DateTimeNull, Is.Null);
                Assert.That(destination2?.String, Is.EqualTo("testMultiple2"));
                Assert.That(destination2?.StringNull, Is.EqualTo("testOverwrite"));
                Assert.That(destination2?.Int, Is.EqualTo(source2.Int + 2));
                Assert.That(destination2?.IntNull, Is.EqualTo(10));
            });
        }

        [Test]
        public void NestMap_Verify()
        {
            //Arrange
            var mappingConfig = new RooMapperManager(x =>
            {
                x.AddProfile(new NestMap());
            });
            var mapper = mappingConfig.CreateMapper();
            var source = new SourceNest() { Nested = new() { String = "test3", Int = 3 }, NestedDifferent = new() { String = "test4", Int = 4 } };
            var destination = new DestinationNest();

            //Act
            destination = mapper.Map<DestinationNest>(source);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(destination?.Nested.String, Is.EqualTo(source.Nested.String));
                Assert.That(destination?.Nested.StringNull, Is.EqualTo(source.Nested.String));
                Assert.That(destination?.Nested.Int, Is.EqualTo(source.Nested.Int + 2));
                Assert.That(destination?.Nested.IntNull, Is.EqualTo(10));
                Assert.That(destination?.NestedNull, Is.Null);
                Assert.That(destination?.NestedDifferent.String, Is.EqualTo(source.NestedDifferent.String));
                Assert.That(destination?.NestedDifferent.StringNull, Is.Null);
                Assert.That(destination?.NestedDifferent.Int, Is.EqualTo(source.NestedDifferent.Int));
                Assert.That(destination?.NestedDifferent.IntNull, Is.Null);
                Assert.That(destination?.NestedDifferentNull, Is.Null);
            });
        }

        [Test]
        public void NestIgnoreMap_Verify()
        {
            //Arrange
            var mappingConfig = new RooMapperManager(x =>
            {
                x.AddProfile(new NestIgnoreMap());
            });
            var mapper = mappingConfig.CreateMapper();
            var source = new SourceNest() { Nested = new() { String = "test3"} };
            var destination = new DestinationNest();

            //Act
            destination = mapper.Map<DestinationNest>(source);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(destination?.Nested.String, Is.EqualTo("test"));
            });
        }

        [Test]
        public void NestedListsMap_Verify()
        {
            //Arrange
            var mappingConfig = new RooMapperManager(x =>
            {
                x.AddProfile(new NestedListsMap());
            });
            var mapper = mappingConfig.CreateMapper();
            var source = new SourceListNest()
            {
                Nested = new() { new() { String = "test3", Int = 3 }, new() { String = "test4", StringNull = "test4", Int = 4, IntNull = 4 } },
                NestedNull = new() { new() { String = "test3" } },
                NestedDifferent = new() { new() { String = "test3", Int = 3, IntNull = 3 }, new() { String = "test4", Int = 4, IntNull = 4 } },
                NestedString = { "test5", "test6" }, NestedInt = { 5, 6 }
            };
            var destination = new DestinationListNest();

            //Act
            destination = mapper.Map<DestinationListNest>(source);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(destination?.Nested[0].String, Is.EqualTo(source.Nested[0].String));
                Assert.That(destination?.Nested[0].StringNull, Is.Null);
                Assert.That(destination?.Nested[0].Int, Is.EqualTo(source.Nested[0].Int));
                Assert.That(destination?.Nested[0].IntNull, Is.Null);
                Assert.That(destination?.Nested[1].String, Is.EqualTo(source.Nested[1].String));
                Assert.That(destination?.Nested[1].StringNull, Is.EqualTo(source.Nested[1].String));
                Assert.That(destination?.Nested[1].Int, Is.EqualTo(source.Nested[1].Int));
                Assert.That(destination?.Nested[1].IntNull, Is.EqualTo(source.Nested[1].IntNull));
                Assert.That(destination?.NestedNull?[0].String, Is.EqualTo(source.NestedNull[0].String));
                Assert.That(destination?.NestedDifferent[0].String, Is.EqualTo(source.NestedDifferent[0].String));
                Assert.That(destination?.NestedDifferent[0].StringNull, Is.EqualTo(source.NestedDifferent[0].String));
                Assert.That(destination?.NestedDifferent[0].Int, Is.EqualTo(2));
                Assert.That(destination?.NestedDifferent[0].IntNull, Is.EqualTo(source.NestedDifferent[0].IntNull));
                Assert.That(destination?.NestedDifferent[1].String, Is.EqualTo(source.NestedDifferent[1].String));
                Assert.That(destination?.NestedDifferent[1].StringNull, Is.EqualTo(source.NestedDifferent[1].String));
                Assert.That(destination?.NestedDifferent[1].Int, Is.EqualTo(2));
                Assert.That(destination?.NestedDifferent[1].IntNull, Is.EqualTo(source.NestedDifferent[1].IntNull));
                Assert.That(destination?.NestedDifferentNull, Is.Null);
                Assert.That(destination?.NestedString?[0], Is.EqualTo(source.NestedString[0]));
                Assert.That(destination?.NestedString?[1], Is.EqualTo(source.NestedString[1]));
                Assert.That(destination?.NestedInt?[0], Is.EqualTo(source.NestedInt[0]));
                Assert.That(destination?.NestedInt?[1], Is.EqualTo(source.NestedInt[1]));
            });
        }

        [Test]
        public void ListOfNestedListsMap_Verify()
        {
            //Arrange
            var mappingConfig = new RooMapperManager(x =>
            {
                x.AddProfile(new NestedListsMap());
            });
            var mapper = mappingConfig.CreateMapper();
            var source = new List<SourceListNest>()
            {
                new()
                {
                    Nested = new() { new() { String = "test3", Int = 3 }, new() { String = "test4", StringNull = "test4", Int = 4, IntNull = 4 } },
                    NestedNull = new() { new() { String = "test3" } },
                    NestedDifferent = new() { new() { String = "test5", Int = 5, IntNull = 6 }, new() { String = "test6", Int = 6, IntNull = 6 } },
                    NestedString = { "test5", "test6" },
                    NestedInt = { 5, 6 }
                },
                new()
                {
                    Nested = new() { new() { String = "test7", Int = 7 } },
                    NestedDifferent = new() { new() { String = "test8", Int = 8 } },
                    NestedString = { "test9", "test10" },
                    NestedInt = { 9, 10 }
                },
            };
            var destination = new List<DestinationListNest>();

            //Act
            destination = mapper.Map<List<DestinationListNest>>(source);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(destination?[0].Nested[0].String, Is.EqualTo(source[0].Nested[0].String));
                Assert.That(destination?[0].Nested[0].StringNull, Is.Null);
                Assert.That(destination?[0].Nested[0].Int, Is.EqualTo(source[0].Nested[0].Int));
                Assert.That(destination?[0].Nested[0].IntNull, Is.Null);
                Assert.That(destination?[0].Nested[1].String, Is.EqualTo(source[0].Nested[1].String));
                Assert.That(destination?[0].Nested[1].StringNull, Is.EqualTo(source[0].Nested[1].String));
                Assert.That(destination?[0].Nested[1].Int, Is.EqualTo(source[0].Nested[1].Int));
                Assert.That(destination?[0].Nested[1].IntNull, Is.EqualTo(source[0].Nested[1].IntNull));
                Assert.That(destination?[0].NestedNull?[0].String, Is.EqualTo(source[0].NestedNull?[0].String));
                Assert.That(destination?[0].NestedDifferent[0].String, Is.EqualTo(source[0].NestedDifferent[0].String));
                Assert.That(destination?[0].NestedDifferent[0].StringNull, Is.EqualTo(source[0].NestedDifferent[0].String));
                Assert.That(destination?[0].NestedDifferent[0].Int, Is.EqualTo(2));
                Assert.That(destination?[0].NestedDifferent[0].IntNull, Is.EqualTo(source[0].NestedDifferent[0].IntNull));
                Assert.That(destination?[0].NestedDifferent[1].String, Is.EqualTo(source[0].NestedDifferent[1].String));
                Assert.That(destination?[0].NestedDifferent[1].StringNull, Is.EqualTo(source[0].NestedDifferent[1].String));
                Assert.That(destination?[0].NestedDifferent[1].Int, Is.EqualTo(2));
                Assert.That(destination?[0].NestedDifferent[1].IntNull, Is.EqualTo(source[0].NestedDifferent[1].IntNull));
                Assert.That(destination?[0].NestedDifferentNull, Is.Null);
                Assert.That(destination?[0].NestedString?[0], Is.EqualTo(source[0].NestedString[0]));
                Assert.That(destination?[0].NestedString?[1], Is.EqualTo(source[0].NestedString[1]));
                Assert.That(destination?[0].NestedInt?[0], Is.EqualTo(source[0].NestedInt[0]));
                Assert.That(destination?[0].NestedInt?[1], Is.EqualTo(source[0].NestedInt[1]));
                Assert.That(destination?[1].Nested[0].String, Is.EqualTo(source[1].Nested[0].String));
                Assert.That(destination?[1].Nested[0].StringNull, Is.Null);
                Assert.That(destination?[1].Nested[0].Int, Is.EqualTo(source[1].Nested[0].Int));
                Assert.That(destination?[1].Nested[0].IntNull, Is.Null);
                Assert.That(destination?[1].NestedDifferent[0].String, Is.EqualTo(source[1].NestedDifferent[0].String));
                Assert.That(destination?[1].NestedDifferent[0].StringNull, Is.EqualTo(source[1].NestedDifferent[0].String));
                Assert.That(destination?[1].NestedDifferent[0].Int, Is.EqualTo(2));
                Assert.That(destination?[1].NestedDifferent[0].IntNull, Is.EqualTo(source[1].NestedDifferent[0].IntNull));
            });
        }

        [Test]
        public void TupleMap_Verify()
        {
            //Arrange
            var mappingConfig = new RooMapperManager(x =>
            {
                x.AddProfile(new TupleMap());
            });
            var mapper = mappingConfig.CreateMapper();
            var sourceString = new SourceTupleString() { String = "test3", StringNull = "test3" };
            var sourceNumbers = new SourceTupleNumbers() { Int = 3, Double = 3, DoubleNull = 3, Decimal = 3 };
            var sourceOthers = new SourceTupleOthers() { Bool = true, BoolNull = true, Byte = 3, DateTime = DateTime.Now.Date };
            var destination = new Destination();

            //Act
            destination = mapper.Map<Destination>((sourceString, sourceNumbers, sourceOthers));

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(destination?.String, Is.EqualTo(sourceString.String));
                Assert.That(destination?.StringNull, Is.EqualTo(sourceString.String + sourceNumbers.Int.ToString()));
                Assert.That(destination?.Int, Is.EqualTo(sourceNumbers.Int + sourceNumbers.Int));
                Assert.That(destination?.IntNull, Is.Null);
                Assert.That(destination?.Double, Is.EqualTo(sourceNumbers.Double));
                Assert.That(destination?.DoubleNull, Is.EqualTo(10));
                Assert.That(destination?.Decimal, Is.EqualTo(10m));
                Assert.That(destination?.DecimalNull, Is.Null);
                Assert.That(destination?.Bool, Is.EqualTo(sourceOthers.Bool));
                Assert.That(destination?.BoolNull, Is.EqualTo(sourceOthers.Bool));
                Assert.That(destination?.Byte, Is.EqualTo(sourceOthers.Byte));
                Assert.That(destination?.ByteNull, Is.Null);
                Assert.That(destination?.DateTime, Is.EqualTo(DateTime.Now.Date.AddDays(-10)));
                Assert.That(destination?.DateTimeNull, Is.EqualTo(DateTime.Now.Date.AddDays(-5)));
            });
        }

        #region MapClasses
        private class AutoReverseMap : Profile
        {
            public AutoReverseMap()
            {
                CreateMap<Source, Destination>().ReverseMap();
            }
        }

        private class CustomMap : Profile
        {
            public CustomMap()
            {
                CreateMap<Source, Destination>()
                    .ForMember(x => x.String, y => y.MapFrom(z => z.String))
                    .ForMember(x => x.StringNull, y => y.MapFrom(z => z.String))
                    .ForMember(x => x.Int, y => y.MapFrom(z => z.Int))
                    .ForMember(x => x.IntNull, y => y.MapFrom(z => z.Int))
                    .ForMember(x => x.Double, y => y.MapFrom(z => z.Double))
                    .ForMember(x => x.DoubleNull, y => y.MapFrom(z => z.Double))
                    .ForMember(x => x.Decimal, y => y.MapFrom(z => z.Decimal))
                    .ForMember(x => x.DecimalNull, y => y.MapFrom(z => z.Decimal))
                    .ForMember(x => x.Bool, y => y.MapFrom(z => z.Bool))
                    .ForMember(x => x.BoolNull, y => y.MapFrom(z => z.Bool))
                    .ForMember(x => x.Byte, y => y.MapFrom(z => z.Byte))
                    .ForMember(x => x.ByteNull, y => y.MapFrom(z => z.Byte))
                    .ForMember(x => x.DateTime, y => y.MapFrom(z => z.DateTime))
                    .ForMember(x => x.DateTimeNull, y => y.MapFrom(z => z.DateTime))
                    .ReverseMap();
            }
        }

        private class IgnoreMap : Profile
        {
            public IgnoreMap()
            {
                CreateMap<Source, Destination>()
                    .ForMember(x => x.String, y => y.Ignore())
                    .ForMember(x => x.StringNull, y => y.MapFrom(z => z.String))
                    .ForMember(x => x.Int, y => y.Ignore())
                    .ForMember(x => x.IntNull, y => y.MapFrom(z => z.Int))
                    .ForMember(x => x.Double, y => y.Ignore())
                    .ForMember(x => x.DoubleNull, y => y.MapFrom(z => z.Double))
                    .ForMember(x => x.Decimal, y => y.Ignore())
                    .ForMember(x => x.DecimalNull, y => y.MapFrom(z => z.Decimal))
                    .ForMember(x => x.Bool, y => y.Ignore())
                    .ForMember(x => x.BoolNull, y => y.MapFrom(z => z.Bool))
                    .ForMember(x => x.Byte, y => y.Ignore())
                    .ForMember(x => x.ByteNull, y => y.MapFrom(z => z.Byte))
                    .ForMember(x => x.DateTime, y => y.Ignore())
                    .ForMember(x => x.DateTimeNull, y => y.MapFrom(z => z.DateTime))
                    .ReverseMap();
            }
        }

        private class ConstantMap : Profile
        {
            public ConstantMap()
            {
                CreateMap<Source, Destination>()
                    .ForMember(x => x.String, y => y.MapFrom(z => "testing"))
                    .ForMember(x => x.StringNull, y => y.MapFrom(z => DateTime.Now.Date.ToString()))
                    .ForMember(x => x.Int, y => y.MapFrom(z => 3))
                    .ForMember(x => x.IntNull, y => y.MapFrom(z => 3))
                    .ForMember(x => x.Double, y => y.MapFrom(z => 1.1))
                    .ForMember(x => x.DoubleNull, y => y.MapFrom(z => 1.1))
                    .ForMember(x => x.Decimal, y => y.MapFrom(z => 1.1m))
                    .ForMember(x => x.DecimalNull, y => y.MapFrom(z => 1.1m))
                    .ForMember(x => x.Bool, y => y.MapFrom(z => true))
                    .ForMember(x => x.BoolNull, y => y.MapFrom(z => true))
                    .ForMember(x => x.Byte, y => y.MapFrom(z => 3))
                    .ForMember(x => x.ByteNull, y => y.MapFrom(z => 3))
                    .ForMember(x => x.DateTime, y => y.MapFrom(z => DateTime.Now.Date.AddDays(-2)))
                    .ForMember(x => x.DateTimeNull, y => y.MapFrom(z => DateTime.Now.Date))
                    .ReverseMap();
            }
        }

        private class ComputedMap : Profile
        {
            public ComputedMap()
            {
                CreateMap<Source, Destination>()
                    .ForMember(x => x.String, y => y.MapFrom(z => z.String + z.String))
                    .ForMember(x => x.StringNull, y => y.MapFrom(z => $"{z.String} testing"))
                    .ForMember(x => x.Int, y => y.MapFrom(z => z.Int + z.Int + z.Int))
                    .ForMember(x => x.IntNull, y => y.MapFrom(z => (int)z.Double))
                    .ForMember(x => x.Double, y => y.MapFrom(z => z.Double * 2))
                    .ForMember(x => x.DoubleNull, y => y.MapFrom(z => z.Double + 0.1))
                    .ForMember(x => x.Decimal, y => y.MapFrom(z => z.Decimal + z.Decimal))
                    .ForMember(x => x.DecimalNull, y => y.MapFrom(z => z.DecimalNull ?? 0.5m))
                    .ForMember(x => x.Bool, y => y.MapFrom(z => z.Bool || true))
                    .ForMember(x => x.BoolNull, y => y.MapFrom(z => z.BoolNull ?? true))
                    .ForMember(x => x.Byte, y => y.MapFrom(z => 0))
                    .ForMember(x => x.ByteNull, y => y.MapFrom(z => z.ByteNull ?? z.Byte))
                    .ForMember(x => x.DateTime, y => y.MapFrom(z => DateTime.Now.Date.AddDays(-1)))
                    .ForMember(x => x.DateTimeNull, y => y.MapFrom(z => DateTime.Now.Date.ToString().ToDateTime()))
                    .ReverseMap();
            }
        }

        private class ListMap : Profile
        {
            public ListMap()
            {
                CreateMap<Source, Destination>()
                    .ForMember(x => x.StringNull, y => y.MapFrom(z => z.String))
                    .ForMember(x => x.IntNull, y => y.MapFrom(z => z.Int))
                    .ForMember(x => x.Double, y => y.Ignore())
                    .ForMember(x => x.DecimalNull, y => y.MapFrom(z => z.Decimal))
                    .ForMember(x => x.BoolNull, y => y.MapFrom(z => z.Bool))
                    .ForMember(x => x.ByteNull, y => y.MapFrom(z => z.Byte))
                    .ForMember(x => x.DateTimeNull, y => y.MapFrom(z => z.DateTime))
                    .ReverseMap();
            }
        }

        private class MultipleProfilesMap : Profile
        {
            public MultipleProfilesMap()
            {
                CreateMap<Source, Destination>()
                    .ForMember(x => x.StringNull, y => y.MapFrom(z => z.String))
                    .ForMember(x => x.Int, y => y.MapFrom(z => z.Int + 2))
                    .ReverseMap();

                CreateMap<Source2, Destination2>()
                    .ForMember(x => x.String, y => y.Ignore())
                    .ForMember(x => x.StringNull, y => y.MapFrom(z => "testOverwrite"))
                    .ForMember(x => x.Int, y => y.MapFrom(z => z.Int + 2))
                    .ForMember(x => x.IntNull, y => y.MapFrom(z => z.IntNull ?? 10))
                    .ReverseMap();
            }
        }

        private class NestMap : Profile
        {
            public NestMap()
            {
                CreateMap<SourceNested, DestinationNested>().ReverseMap();

                CreateMap<SourceNest, DestinationNest>()
                    .ForMember(x => x.Nested.StringNull, y => y.MapFrom(z => z.Nested.String))
                    .ForMember(x => x.Nested.Int, y => y.MapFrom(z => z.Nested.Int + 2))
                    .ForMember(x => x.Nested.IntNull, y => y.MapFrom(z => 10))
                    .ReverseMap();
            }
        }

        private class NestIgnoreMap : Profile
        {
            public NestIgnoreMap()
            {
                CreateMap<SourceNest, DestinationNest>()
                    .ForMember(x => x.Nested.String, y => y.Ignore())
                    .ReverseMap();
            }
        }

        private class NestedListsMap : Profile
        {
            public NestedListsMap()
            {
                CreateMap<SourceNested, DestinationNested>()
                    .ForMember(x => x.StringNull, y => y.MapFrom(z => z.String))
                    .ForMember(x => x.Int, y => y.Ignore())
                    .ReverseMap();

                CreateMap<SourceListNest, DestinationListNest>().ReverseMap();
            }
        }

        private class TupleMap : Profile
        {
            public TupleMap()
            {
                CreateMap<(SourceTupleString SourceTupleString, SourceTupleNumbers SourceTupleNumbers, SourceTupleOthers SourceTupleOthers), Destination>()
                    .ForMember(x => x.String, y => y.MapFrom(z => z.SourceTupleString.String))
                    .ForMember(x => x.StringNull, y => y.MapFrom(z => z.SourceTupleString.String + z.SourceTupleNumbers.Int.ToString()))
                    .ForMember(x => x.Int, y => y.MapFrom(z => z.SourceTupleNumbers.Int + z.SourceTupleNumbers.Int))
                    .ForMember(x => x.Double, y => y.MapFrom(z => z.SourceTupleNumbers.Double))
                    .ForMember(x => x.DoubleNull, y => y.MapFrom(z => 10))
                    .ForMember(x => x.Decimal, y => y.MapFrom(z => z.SourceTupleNumbers.DecimalNull ?? 10m))
                    .ForMember(x => x.DecimalNull, y => y.MapFrom(z => z.SourceTupleNumbers.DecimalNull))
                    .ForMember(x => x.Bool, y => y.MapFrom(z => z.SourceTupleOthers.Bool))
                    .ForMember(x => x.BoolNull, y => y.MapFrom(z => z.SourceTupleOthers.BoolNull))
                    .ForMember(x => x.Byte, y => y.MapFrom(z => z.SourceTupleOthers.Byte))
                    .ForMember(x => x.DateTime, y => y.MapFrom(z => DateTime.Now.Date.AddDays(-10)))
                    .ForMember(x => x.DateTimeNull, y => y.MapFrom(z => z.SourceTupleOthers.DateTimeNull ?? DateTime.Now.Date.AddDays(-5)))
                    .ReverseMap();
            }
        }
        #endregion

        #region Classes
        private class Source()
        {
            public string String { get; set; } = "test";
            public string? StringNull { get; set; }
            public int Int { get; set; } = 1;
            public int? IntNull { get; set; }
            public double Double { get; set; } = 1.0;
            public double? DoubleNull { get; set; }
            public decimal Decimal { get; set; } = 1.0m;
            public decimal? DecimalNull { get; set; }
            public bool Bool { get; set; } = true;
            public bool? BoolNull { get; set; }
            public byte Byte { get; set; } = 1;
            public byte? ByteNull { get; set; }
            public DateTime DateTime { get; set; } = DateTime.Now.Date;
            public DateTime? DateTimeNull { get; set; }
        }

        private class Destination()
        {
            public string String { get; set; } = "test2";
            public string? StringNull { get; set; }
            public int Int { get; set; } = 2;
            public int? IntNull { get; set; }
            public double Double { get; set; } = 2.0;
            public double? DoubleNull { get; set; }
            public decimal Decimal { get; set; } = 2.0m;
            public decimal? DecimalNull { get; set; }
            public bool Bool { get; set; } = false;
            public bool? BoolNull { get; set; }
            public byte Byte { get; set; } = 2;
            public byte? ByteNull { get; set; }
            public DateTime DateTime { get; set; } = DateTime.Now.Date.AddDays(1);
            public DateTime? DateTimeNull { get; set; }
        }

        private class Source2()
        {
            public string String { get; set; } = "testMultiple";
            public string? StringNull { get; set; }
            public int Int { get; set; } = 3;
            public int? IntNull { get; set; }
        }

        private class Destination2()
        {
            public string String { get; set; } = "testMultiple2";
            public string? StringNull { get; set; }
            public int Int { get; set; } = 4;
            public int? IntNull { get; set; }
        }

        private class SourceNest()
        {
            public SourceNested Nested { get; set; } = new();
            public SourceNested? NestedNull { get; set; }
            public SourceNested NestedDifferent { get; set; } = new();
            public SourceNested? NestedDifferentNull { get; set; }
        }

        private class DestinationNest()
        {
            public SourceNested Nested { get; set; } = new();
            public SourceNested? NestedNull { get; set; }
            public DestinationNested NestedDifferent { get; set; } = new();
            public DestinationNested? NestedDifferentNull { get; set; }
        }

        private class SourceListNest()
        {
            public List<SourceNested> Nested { get; set; } = new() { new() };
            public List<SourceNested>? NestedNull { get; set; }
            public List<SourceNested> NestedDifferent { get; set; } = new() { new() };
            public List<SourceNested>? NestedDifferentNull { get; set; }
            public List<string> NestedString { get; set; } = new();
            public List<int> NestedInt { get; set; } = new();
        }

        private class DestinationListNest()
        {
            public List<SourceNested> Nested { get; set; } = new() { new() };
            public List<SourceNested>? NestedNull { get; set; }
            public List<DestinationNested> NestedDifferent { get; set; } = new() { new() };
            public List<DestinationNested>? NestedDifferentNull { get; set; }
            public List<string>? NestedString { get; set; }
            public List<int>? NestedInt { get; set; }
        }

        private class SourceNested()
        {
            public string String { get; set; } = "test";
            public string? StringNull { get; set; }
            public int Int { get; set; } = 1;
            public int? IntNull { get; set; }
        }

        private class DestinationNested()
        {
            public string String { get; set; } = "test2";
            public string? StringNull { get; set; }
            public int Int { get; set; } = 2;
            public int? IntNull { get; set; }
        }

        private class SourceTupleString()
        {
            public string String { get; set; } = "test";
            public string? StringNull { get; set; }
        }

        private class SourceTupleNumbers()
        {
            public int Int { get; set; } = 1;
            public int? IntNull { get; set; }
            public double Double { get; set; } = 1.0;
            public double? DoubleNull { get; set; }
            public decimal Decimal { get; set; } = 1.0m;
            public decimal? DecimalNull { get; set; }
        }

        private class SourceTupleOthers()
        {
            public bool Bool { get; set; } = true;
            public bool? BoolNull { get; set; }
            public byte Byte { get; set; } = 1;
            public byte? ByteNull { get; set; }
            public DateTime DateTime { get; set; } = DateTime.Now.Date;
            public DateTime? DateTimeNull { get; set; }
        }
        #endregion
    }
}