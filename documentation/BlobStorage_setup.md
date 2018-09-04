
Installering af Azure Blob Storage
================================

Vi ønsker at Media og alle cached items (altså behandlet med imageprocessor) bliver stored i en Azure Blob storage. 

#### Bemærkninger : 
- Hver kunde skal have sin egen storage blob
  - navngivet efter formatet skybrudstorage0000xx
  - hvis projektet ikke har en Resource group, oprettes en efter formatet skybrud0000xx
- Billederne skal ligge i en container som hedder media
- Cache skal ligge i en container som hedder cache
- Billeder fra eksisterende løsninger flyttes nemt med "Microsoft azure storage explorer"(MASE). Til store fil mængder kan REDGATE's freeware program også benyttes.
- Ved SPA løsning kan der være en fejl ifht. brugen af web.config.base da en af pakkerne tilføjer en dependcy binding dette skal tilføjes web.config.base (https://github.com/JimBobSquarePants/UmbracoFileSystemProviders.Azure/issues/80)

#### Installations process : 

1. Installer: 
    Install-Package UmbracoFileSystemProviders.Azure web
    
   (info her https://github.com/JimBobSquarePants/UmbracoFileSystemProviders.Azure)

2. Opsætning af /config/FileSystemProvider.config
------
```
<?xml version="1.0"?>
<FileSystemProviders>
  
  <!-- Media -->
  <Provider alias="media" type="Our.Umbraco.FileSystemProviders.Azure.AzureBlobFileSystem, Our.Umbraco.FileSystemProviders.Azure">
  <Parameters>
  <add key="containerName" value="media"/>
  <add key="rootUrl" value="https://{ACCOUNTNAME}.blob.core.windows.net/"/>
  <add key="connectionString" value="DefaultEndpointsProtocol=https;AccountName={ACCOUNTNAME};AccountKey={KEY}"/>
  <!--
        Optional configuration value determining the maximum number of days to cache items in the browser.
        Defaults to 365 days.
      -->
  <add key="maxDays" value="365"/>
  <!--
        When true this allows the VirtualPathProvider to use the deafult "media" route prefix regardless 
        of the container name.
      -->
  		<add key="useDefaultRoute" value="true"/>
  </Parameters>
  </Provider>
   
</FileSystemProviders>
```
------

3. Installer 2 pakker mere :
```
Install-Package ImageProcessor.Web.Config web
Install-Package ImageProcessor.Web.Plugins.AzureBlobCache web
```

4. Opsætning af configs 

/Config/Imageprocessor/security.config:
Services ser sådan her ud, skift host ud til kundens

```
   <?xml version="1.0" encoding="utf-8"?>
<security>
  <services>
    <service  prefix="media/" name="CloudImageService" type="ImageProcessor.Web.Services.CloudImageService, ImageProcessor.Web">
      <settings>
        <setting key="Container" value="media"/>
        <setting key="MaxBytes" value="8194304"/>
        <setting key="Timeout" value="30000"/>
        <setting key="Host" value="https://{ACCOUNTNAME}.blob.core.windows.net/"/>
      </settings>
    </service>
    
  </services>
</security> 
```

/Config/Imageprocessor/cache.config:
Skift storage account til kundes

```
<caching currentCache="AzureBlobCache">
  <caches>
  <!--
    <cache name="DiskCache" type="ImageProcessor.Web.Caching.DiskCache, ImageProcessor.Web" maxDays="365">
      <settings>
        <setting key="VirtualCachePath" value="~/app_data/cache" />
      </settings>
    </cache>
     -->
  <cache name="AzureBlobCache" type="ImageProcessor.Web.Plugins.AzureBlobCache.AzureBlobCache, ImageProcessor.Web.Plugins.AzureBlobCache" maxDays="365">
      <settings>
        <setting key="CachedStorageAccount" value="DefaultEndpointsProtocol=https;AccountName={ACCOUNTNAME};AccountKey={KEY}" />
        <setting key="CachedBlobContainer" value="cache" />
        <setting key="UseCachedContainerInUrl" value="true" />
        <setting key="SourceStorageAccount" value="DefaultEndpointsProtocol=https;AccountName={ACCOUNTNAME};AccountKey={KEY}" />
        <setting key="SourceBlobContainer" value="media" />
        <setting key="StreamCachedImage" value="false" />
      </settings>
    </cache></caches>
</caching>
```

Tjek om følgende er tilføjet i din web.base.config (den skal ligge under /runtime/assemblyBinding/

```
<dependentAssembly>
        <assemblyIdentity name="Microsoft.WindowsAzure.Storage" publicKeyToken="31bf3856ad364e35" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-8.7.0.0" newVersion="8.7.0.0" />
      </dependentAssembly>
```

Test lokalt, se med MASE at det virker og folderne bliver populated.