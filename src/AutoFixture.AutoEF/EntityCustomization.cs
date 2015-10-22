using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using System;

namespace AutoFixture.AutoEF
{
    public class EntityCustomization : ICustomization
    {
        private readonly IEntityTypesProvider _entityTypesProvider;

        public EntityCustomization(IEntityTypesProvider entityTypesProvider)
        {
            if (entityTypesProvider == null)
                throw new ArgumentNullException("entityTypesProvider");

            _entityTypesProvider = entityTypesProvider;
        }

        public void Customize(IFixture fixture)
        {
            var cachedTypesProvider = new CachedEntityTypesProvider(_entityTypesProvider);

            fixture.Customizations.Insert(0, new AutoEntitySpecimenBuilder(cachedTypesProvider));
        }
    }

    public class AutoEntitySpecimenBuilder : FilteringSpecimenBuilder
    {
        public AutoEntitySpecimenBuilder(IEntityTypesProvider entityTypesProvider)
            : base(
                new Postprocessor(
                    new EntitySpecimenBuilder(),
                    new AutoPropertiesCommand(
                        new InverseRequestSpecification(
                            new OrRequestSpecification(
                                new NavigationPropertyRequestSpecification(entityTypesProvider),
                                new InterceptorsFieldRequestSpecification())))),
                new EntityRequestSpecification(entityTypesProvider))
        { }
    }
}