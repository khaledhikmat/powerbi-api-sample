# powerbi-api-sample

[PowerBI](https://msdn.microsoft.com/powerbi) is Microsoft's new BI offering. In this sample, I show how to use the APIs to create and populate data sets. This allows developers to update PowerBI dashboards in real-time.

To learn more about PowerBI, please visit the [development center](https://msdn.microsoft.com/en-us/library/dn877544.aspx) to get an introduction to the PowerBI APIs. The site also explains how to register your client app in Azure Active Directory so it can authenticate against AD.

One thing is worth mentioning here is that PowerBI is currently available with Office365 subscription and in the United States only. If you happen to have an Azure subscription that was created using a Microsoft account (not a company account) and you want to use PowerBI, you must first import your company (Office 365) Active Directory into your Azure subscription. I struggled with this but it turned out there is a solution. Please refer to this forum [thread](https://social.msdn.microsoft.com/Forums/en-US/c4e3d746-8dc0-4249-b954-f2cc49b61f2a/how-to-associate-an-existing-microsoft-accountbased-azure-subscription-with-my-organization-in?forum=WindowsAzureAD).
  
## The Solution

Using VS2013, created a Console Windows application.

## Nuget Packages:

The following Nuget packages are required:

```
Install-Package Newtonsoft.Json
Install-Package Microsoft.Net.Http
Install-Package Microsoft.IdentityModel.Clients.ActiveDirectory
```

## Models

The following classes are used to model the PowerBI API JSON structures:

* Column
* Dataset
* DatasetList
* Table 
* TableRows

The following classes are used to model example data set tables:

* Sales
* TableBookings

## What it does:

The program is very simple! It does the following things:

* Presents a login form so the user can login to the Azure AD. The user must have signed up for PowerBI.

![](http://i.imgur.com/ow8LHEg.png)

* Gets the current list of data sets available in the authenticated account
* Creates a new data set with two tables: Sales and Table Bookings
* Re-gets the list of data sets
* Adds rows to the newly created dataset

![](http://i.imgur.com/flvWQMX.png)

Once the rows are added, you can visualize the data using the dashboard:

![](http://i.imgur.com/yo8ZW2x.png)

## Real-Time

You can easily add more rows to the data set and watch the dashboard react in real-time .....really fantastic. 

## Caveats

The sample program uses a console app to demonstrate the ability to use the APIs to create data sets and add rows. But in reality, Console apps cannot be used easily in production. In this preview version of PowerBI, I could not find a way to authenticate against PowerBI using a daemon process (unattended mode). I want to be able to write rows in a data set from an Azure Web Job, for example. 

This [blog post](http://blogs.msdn.com/b/powerbidev/archive/2014/12/23/power-bi-apis-real-time-example.aspx) by a PowerBI engineer uses a console app to authenticate against AD, create the data set and then launches self-hosted HTTP listener to write rows from POST requests.
