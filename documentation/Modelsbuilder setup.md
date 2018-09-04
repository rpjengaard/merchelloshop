# Opsætning af Modelsbuilder #

- Umbraco.ModelsBuilder (NugetPakket) i code projektet `PM> Install-Package Umbraco.ModelsBuilder code`
- Umbraco.ModelsBuilder.Api (NugetPakke) `PM> Install-Package Umbraco.ModelsBuilder.Api web`
  - Sæt web.config options til følgende:
    - Umbraco.ModelsBuilder.Enable : true
    - Umbraco.ModelsBuilder.ModelsMode : Nothing
    - Umbraco.ModelsBuilder.EnableApi : true
    - Umbraco.ModelsBuilder.LanguageVersion : CSharp6
    - OBS: Husk at sætte debug til true, ellers vil det ikke virke.
- Umbraco ModelsBuilder Custom Tool (VS pakke, installeres under Tools> Extensions and tools)
  - Opsætning af Umbraco ModelsBuilder Custom Tool
    - Dette gøres under `Tools` > `Options` > `Umbraco` > `ModelsBuilder Options`
    - Sæt her url (på din lokale løsning), username (umbraco bruger), password (umbraco bruger)
    - Opret en klasse med navnet "Builder" under samme namespace som restende page models (eks. billundkommune.Models.Website)    
    - Højre klik på klassen, tryk properties, skriv "UmbracoModelsBuilder" i feltet Custom Tool
    - Herefter kan du nu højre klikke på "Builder.cs" og trykke "Run Custom Tool"
