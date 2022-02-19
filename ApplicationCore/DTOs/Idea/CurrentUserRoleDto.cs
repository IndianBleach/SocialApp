using ApplicationCore.Entities.IdeaEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.Idea
{
    public enum CurrentUserRoleTypes
    { 
        Author,
        Modder,
        Member,
        Viewer
    }

    public class CurrentUserRoleDto
    {
        public string RoleName { get; set; }
        public CurrentUserRoleTypes Role { get; set; }

        public CurrentUserRoleDto(IdeaMember? member)
        {
            if (member != null)
            {
                switch (member.Role)
                {
                    case IdeaMemberRoles.Author:
                        Role = CurrentUserRoleTypes.Author;
                        RoleName = "Автор";
                        break;

                    case IdeaMemberRoles.Modder:
                        Role = CurrentUserRoleTypes.Modder;
                        RoleName = "Модератор";
                        break;

                    case IdeaMemberRoles.Member:
                        Role = CurrentUserRoleTypes.Member;
                        RoleName = "Участник";
                        break;

                    default:
                        Role = CurrentUserRoleTypes.Viewer;
                        RoleName = "Наблюдатель";
                        break;
                }
            }
            else
            {                
                Role = CurrentUserRoleTypes.Viewer;
                RoleName = "Наблюдатель";              
            }
        }

        public CurrentUserRoleDto(IdeaMemberRoles roleBase)
        {
            switch (roleBase)
            {
                case IdeaMemberRoles.Author:
                    Role = CurrentUserRoleTypes.Author;
                    RoleName = "Автор";
                    break;

                case IdeaMemberRoles.Modder:
                    Role = CurrentUserRoleTypes.Modder;
                    RoleName = "Модератор";
                    break;

                case IdeaMemberRoles.Member:
                    Role = CurrentUserRoleTypes.Member;
                    RoleName = "Участник";
                    break;

                default:
                    Role = CurrentUserRoleTypes.Viewer;
                    RoleName = "Наблюдатель";
                    break;
            }
        }
    }
}
