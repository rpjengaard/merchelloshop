# README #


## Server/Client requirements ##

### Backender setup ###
* Windows Server with IIS 8.5+ and ASP.NET 4.5+
* [URL Rewrite](https://www.iis.net/downloads/microsoft/url-rewrite)
* [Latest node.js build for Windows](https://nodejs.org/en/download/)
* [iisnode](https://github.com/tjanczuk/iisnode) (extension for IIS)
* [+ Umbraco requirements](https://our.umbraco.org/documentation/getting-started/setup/install/system-requirements)


### Frontender setup ###
* [Latest yarn package manager](https://yarnpkg.com/)
* [Latest node.js](https://nodejs.org/en/download/)

___


## How do I get set up? ##


### Frontend ###
Make sure you have [yarn](https://yarnpkg.com/) installed. In your favorite bash run the following commands:

*to setup packages + build the solution*
```bash 
yarn build
```
*to run the live server at localhost:8080*
```bash 
yarn start
```
*or run the dev server (with hot module reloading etc.) at localhost:8080*
```bash 
yarn dev
```

**Optional:** You can set up Chrome Node.js Dev Tools to listen to localhost:8080 for better debugging of server-side Node stuff. You do this by going to `about:inspect` in chrome. More details [here](https://medium.com/@paul_irish/debugging-node-js-nightlies-with-chrome-devtools-7c4a1b95ae27).


### Backenders only (IIS / IISNode) ###

You have to setup 2 environments. 

1. **IIS-Node env for SPA Client (eg. 0000xx.rpjengaard)**
    1. If you have installed [iisnode](https://github.com/tjanczuk/iisnode), just add IIS-entry and point it to root folder of repo (where server.js lives), and add your binding (eg. 0000xx.rpjengaard)

2. **IIS entry for API    (eg. 0000xx.api.rpjengaard)**
    1. Add IIS entry and point it to ```Dev\web``` folder and add your binding (0000xx.api.rpjengaard)

After that you need to do the following:

1. Copy the file ```_.env``` and name it ```.env```

2. In ```.env``` change API_DOMAIN and API_HOST to your local endpoint and host.
Ex:
```
API_DOMAIN=http://0000xx.api.rpjengaard
API_HOST=0000xx.rpjengaard
```

3. *Run this command from your bash to setup packages + build the SPA frontend*
```bash
yarn build
```

4. Open Umbraco and add your localhost-api-domain *AND your localhost-client-domain* as bindings

5. Open 0000xx.rpjengaard in your browser

___

## API ##

### Requesting the api ###

The new SPA api is no longer 3 different endpoints. Now its one endpoint where you can choose which parts (content, nav, site) you want the API to return.

At the moment the API handles both Skybrud.Redirects and Umbraco (internal) redirect. If you ex. rename a node.


To get startet use this enpoint: `/umbraco/api/spa/GetData/`




#### Params ####

|Property|Type|Value|
|--------|----|-----|
|siteId|`int`|id of the domaincontainer|
|url|`string`|the current url|
|parts|, seperated string|content,nav,site|
|navLevels|`int`|how many levels to you want in your navigation-part (only children)|
|navContext|`bool`|if you wan�t to fetch a navigation-context object (normaly only on initial request)|




#### Headers ####
As the connection between the frontend nodeJS-app and UmbracoCms is stateless, you need to tell the API what domain and what protocol you are using. This is done by adding these keys in your header of your request.

|Key|Value|Description|
|--------|----|-----|
|appHost|ex. 6197sk.testserver.nu|The domain where the frontend app runs from|
|appProtocol|https/http|The schema/protocol|
|appSiteId|`int`|Id of root node|




#### Data examples ####

*Response with all parts + navContext*
```Json

{
    "id": 1073,
    "siteId": 1057,
    "navigation": {
        "children": [
            {
                "id": 1075,
                "name": "Underside 1.1.1.1",
                "url": "/underside-1/underside-11-ny-3/underside-111/underside-1111/",
                "parentId": 1073,
                "template": "Subpage",
                "hasChildren": false,
                "isVisible": true,
                "children": []
            }
        ],
        "context": {
            "id": 1057,
            "name": "DomainController",
            "url": "/",
            "parentId": -1,
            "template": "",
            "hasChildren": true,
            "isVisible": true,
            "children": [
                {
                    "id": 1062,
                    "name": "Frontpage",
                    "url": "/frontpage/",
                    "parentId": 1057,
                    "template": "Frontpage",
                    "hasChildren": false,
                    "isVisible": true,
                    "children": null
                },
                {
                    "id": 1063,
                    "name": "Underside 1",
                    "url": "/underside-1/",
                    "parentId": 1057,
                    "template": "Subpage",
                    "hasChildren": true,
                    "isVisible": true,
                    "children": [
                        {
                            "id": 1071,
                            "name": "Underside 1.1 ny 3",
                            "url": "/underside-1/underside-11-ny-3/",
                            "parentId": 1063,
                            "template": "Subpage",
                            "hasChildren": true,
                            "isVisible": false,
                            "children": [
                                {
                                    "id": 1073,
                                    "name": "Underside 1.1.1",
                                    "url": "/underside-1/underside-11-ny-3/underside-111/",
                                    "parentId": 1071,
                                    "template": "Subpage",
                                    "hasChildren": true,
                                    "isVisible": true,
                                    "children": null
                                },
                                {
                                    "id": 1074,
                                    "name": "Underside 1.1.2",
                                    "url": "/underside-1/underside-11-ny-3/underside-112/",
                                    "parentId": 1071,
                                    "template": "Subpage",
                                    "hasChildren": false,
                                    "isVisible": true,
                                    "children": null
                                }
                            ]
                        },
                        {
                            "id": 1072,
                            "name": "Underside 1.2",
                            "url": "/underside-1/underside-12/",
                            "parentId": 1063,
                            "template": "Subpage",
                            "hasChildren": true,
                            "isVisible": true,
                            "children": null
                        }
                    ]
                },
                {
                    "id": 1064,
                    "name": "Underside 21",
                    "url": "/underside-21/",
                    "parentId": 1057,
                    "template": "Subpage",
                    "hasChildren": true,
                    "isVisible": true,
                    "children": null
                }
            ]
        }
    },
    "site": null,
    "content": {
        "id": 1073,
        "name": "Underside 1.1.1",
        "url": "/underside-1/underside-11-ny-3/underside-111/",
        "contentTitle": "Underside 1.1.1",
        "contentTeaser": "",
        "contentColor": "",
        "contentIcon": "",
        "noCache": false,
        "contentChangesGuid": "980065a2-45f0-4a79-9dd1-e944b7b744e3",
        "JsChangesGuid": "d14c792d-b80b-4f65-a0bf-0ef9ae6896d9",
        "bodyClass": "",
        "path": [
            1057,
            1063,
            1071,
            1073
        ],
        "templatename": "Subpage",
        "created": "2017-07-14T14:55:09",
        "updated": "2017-07-14T14:55:09",
        "jsonDebug": {}
    },
    "executeTimeMs": 5
}

```


*Response when hitting a redirected url (Skybrud.Redirect or Umbraco.Redirect)*
```Json

{
    "meta": {
        "code": 301,
        "error": "Page has moved"
    },
    "data": null
}

```
Besides the body-json, you will find the url to redirect to in the `location`-header.



*Response when hitting a node not existing*
```Json
{
    "meta": {
        "code": 404,
        "error": "Siden fandtes ikke"
    },
    "data": null
}
```
