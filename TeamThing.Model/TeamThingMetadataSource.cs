using System.Collections.Generic;
using TeamThing.Model.TypeConverters;
using Telerik.OpenAccess.Metadata;
using Telerik.OpenAccess.Metadata.Fluent;

namespace TeamThing.Model
{
    public class TeamThingMetadataSource : FluentMetadataSource
    {
        protected override MetadataContainer CreateModel()
        {
            var model = base.CreateModel();

            model.NameGenerator.UseModelNames = true;
            model.NameGenerator.UseDefaultMapping = true;
            return model;
        }

        protected override IList<MappingConfiguration> PrepareMapping()
        {
            List<MappingConfiguration> configurations = new List<MappingConfiguration>();

            configurations.Add(TeamMapping());
            configurations.Add(TeamUserMapping());
            configurations.Add(ThingMapping());
            configurations.Add(UserThingMapping());
            configurations.Add(ThingLogMapping());
            configurations.Add(UserMapping());

            return configurations;
        }
  
        private MappingConfiguration UserMapping()
        {
            var mapping = new MappingConfiguration<User>();

            mapping.MapType()
                  .ToTable("User");

            mapping.HasProperty(m => m.Id)
                   .IsIdentity(KeyGenerator.Autoinc);

            mapping.HasProperty(m => m.IsActive)
                   .HasColumnType("bit");

            mapping.HasAssociation(m => m.Things)
                   .WithOpposite(m => m.AssignedToUser)
                   .IsManaged()
                   .IsDependent()
                   .ToColumn("AssignedToUserId");
            mapping.HasAssociation(m => m.Teams)
                   .WithOpposite(t => t.User)
                   .IsManaged()
                   .IsDependent()
                   .ToColumn("UserId");
            return mapping;
        }
  
        private MappingConfiguration ThingLogMapping()
        {
            var mapping = new MappingConfiguration<ThingLog>();

            mapping.MapType()
                  .ToTable("ThingLog");

            mapping.HasProperty(m => m.Id)
                   .IsIdentity(KeyGenerator.Autoinc);

            mapping.HasAssociation(m => m.Thing)
                   .WithOpposite(t=>t.History)
                   .ToColumn("ThingId");

            mapping.HasAssociation(m => m.EditedBy)
                   .ToColumn("EditedByUserId");

            mapping.HasProperty(m => m.Action)
                   .HasColumnType("varchar")
                   .HasPrecision(25)
                   .WithConverter<EnumToStringConverter<ThingAction>>();

            return mapping;
        }

        private MappingConfiguration UserThingMapping()
        {
            var mapping = new MappingConfiguration<UserThing>();

            mapping.MapType()
                   .ToTable("UserThing");

           mapping.HasIdentity(map => new
                    {
                        map.AssignedToUserId,
                        map.ThingId,
                    });

            mapping.HasAssociation(m => m.Thing)
                   .IsManaged()
                   .IsDependent()
                   .ToColumn("ThingId");

            mapping.HasAssociation(m => m.AssignedByUser)
                   .ToColumn("AssignedByUserId");

            mapping.HasAssociation(m => m.AssignedToUser)
                   .ToColumn("AssignedToUserId");

            return mapping;
        }

        private MappingConfiguration ThingMapping()
        {
            var mapping = new MappingConfiguration<Thing>();

            mapping.MapType()
                   .ToTable("Thing");

            mapping.HasProperty(m => m.Id)
                   .IsIdentity(KeyGenerator.Autoinc);

            mapping.HasProperty(m => m.IsDeleted)
                   .HasColumnType("bit");

            mapping.HasAssociation(m => m.AssignedTo)
                   .WithOpposite(t => t.Thing)
                   .IsManaged()
                   .IsDependent();

            mapping.HasAssociation(m => m.Owner)
                   .IsManaged()
                   .IsDependent()
                   .ToColumn("OwnerId");

            mapping.HasProperty(m => m.Status)
                   .HasColumnType("varchar")
                   .HasPrecision(25)
                   .WithConverter<EnumToStringConverter<ThingStatus>>();

            return mapping;
        }

        private MappingConfiguration TeamUserMapping()
        {
            var mapping = new MappingConfiguration<TeamUser>();

            mapping.MapType()
                   .ToTable("TeamUser");

            mapping.HasIdentity(map => new
            {
                map.UserId,
                map.TeamId,
            });

            mapping.HasAssociation(m => m.Team)
                   .WithOpposite(t => t.TeamMembers)
                   .IsManaged()
                   .IsDependent()
                   .ToColumn("TeamId");

            mapping.HasAssociation(m => m.User)
                   .WithOpposite(t => t.Teams)
                   .IsManaged()
                   .IsDependent()
                   .ToColumn("UserId");

            mapping.HasProperty(m => m.Status)
                   .HasColumnType("varchar")
                   .HasPrecision(25)
                   .WithConverter<EnumToStringConverter<TeamUserStatus>>();

            mapping.HasProperty(m => m.Role)
                   .HasColumnType("varchar")
                   .HasPrecision(25)
                   .WithConverter<EnumToStringConverter<TeamUserRole>>();

            return mapping;
        }

        private MappingConfiguration TeamMapping()
        {
            var mapping = new MappingConfiguration<Team>();

            mapping.MapType()
                   .ToTable("Team");

            mapping.HasProperty(m => m.Id)
                   .IsIdentity(KeyGenerator.Autoinc);

            mapping.HasProperty(m => m.IsOpen)
                   .HasColumnType("bit");
         

            mapping.HasAssociation(m => m.Owner)
                   .IsManaged()
                   .ToColumn("OwnerId");

            mapping.HasAssociation(m => m.TeamMembers)
                   .WithOpposite(t => t.Team)
                   .IsManaged()
                   .IsDependent()
                   .ToColumn("TeamId");

            return mapping;
        }
    }
}
