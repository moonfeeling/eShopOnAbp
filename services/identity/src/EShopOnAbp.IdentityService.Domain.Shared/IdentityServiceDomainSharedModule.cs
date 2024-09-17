using EShopOnAbp.IdentityService.Localization;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace EShopOnAbp.IdentityService
{
    [DependsOn(
        typeof(AbpIdentityDomainSharedModule)
        )]
    public class IdentityServiceDomainSharedModule : AbpModule
    {
        //以下注释代码多余：IdentityServiceEfCoreEntityExtensionMappings类中有调用，其又在IdentityServiceEntityFrameworkCoreModule模块类中被预配置
        /*public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            IdentityServiceModuleExtensionConfigurator.Configure();
        }*/

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<IdentityServiceDomainSharedModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<IdentityServiceResource>("en")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/Localization/IdentityService");

                options.DefaultResourceType = typeof(IdentityServiceResource);
            });
        }
    }
}
