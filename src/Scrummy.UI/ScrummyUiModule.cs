﻿using Autofac;
using Scrummy.DataAccess.GitLab;
using Scrummy.DataAccess.GitLab.Configs;
using Scrummy.Scrum;
using Scrummy.UI.Configs;

namespace Scrummy.UI
{
    public class ScrummyUiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterTypes(builder);
            RegisterModules(builder);
        }

        private static void RegisterTypes(ContainerBuilder builder)
        {
            builder.RegisterType<GitLabConfig>()
                .As<IGitLabConfig>()
                .SingleInstance();

            builder.RegisterType<ScrumConfig>()
                .As<IScrumConfig>()
                .SingleInstance();
        }
        
        private static void RegisterModules(ContainerBuilder builder)
        {
            builder.RegisterModule<ScrumModule>();
            builder.RegisterModule<GitLabModule>();
        }
    }
}