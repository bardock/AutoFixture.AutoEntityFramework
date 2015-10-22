using System.Linq;
using AutoFixture.AutoEF.Tests.MockEntities;
using FluentAssertions;
using Ploeh.AutoFixture;
using Xunit;

namespace AutoFixture.AutoEF.Tests
{
    public class CustomizationComposerIntegrationTest
    {
        [Theory, AutoEFData(typeof(MockDbContext))]
        public void CustomizeEntity(IFixture fixture, string expectedName)
        {
            var autoEfSpecimenBuilder = fixture.Customizations.First(x => x is AutoEntitySpecimenBuilder);
            fixture.Customize<Foo>(c => c
                .FromFactory(autoEfSpecimenBuilder)
                .OmitAutoProperties()
                .With(x => x.Name, expectedName));

            var foo = fixture.Create<Foo>();

            foo.Bar.Foo.Should().Be(foo);
            foo.Name.Should().Be(expectedName);
        }

        [Theory, AutoEFData(typeof(MockDbContext))]
        public void CustomizeRelatedEntity(IFixture fixture, string expectedName)
        {
            fixture.Customize<Bar>(c => c.With(x => x.Name, expectedName));

            var foo = fixture.Create<Foo>();

            foo.Bar.Foo.Should().Be(foo);
            foo.Bar.Name.Should().Be(expectedName);
        }

        [Theory, AutoEFData(typeof(MockDbContext))]
        public void CustomizeRelatedCollectionOfEntities(IFixture fixture, string expectedName)
        {
            fixture.Customize<Qux>(c => c.With(x => x.Name, expectedName));

            var bar = fixture.Create<Bar>();

            bar.Foo.Bar.Should().Be(bar);
            bar.Quxes.Should().NotBeEmpty();
            bar.Quxes.ToList().ForEach(q =>
            {
                q.Bar.Should().Be(bar);
                q.Name.Should().Be(expectedName);
            });
        }

        [Theory, AutoEFData(typeof(MockDbContext))]
        public void BuildEntity(IFixture fixture, string expectedName)
        {
            var autoEfSpecimenBuilder = fixture.Customizations.First(x => x is AutoEntitySpecimenBuilder);
            var fooBuilder = fixture.Build<Foo>()
                .FromFactory(autoEfSpecimenBuilder)
                .OmitAutoProperties()
                .With(x => x.Name, expectedName);

            var foo = fooBuilder.Create();

            foo.Bar.Foo.Should().Be(foo);
            foo.Name.Should().Be(expectedName);
        }
    }
}