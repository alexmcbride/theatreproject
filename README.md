# Local Theatre Company Project

College project to develop web site for local theatre, written in C# and ASP.NET MVC. 

To run the site the following file needs to be placed in the main directory (alongside Web.config):

`Secret.config`

This file should contain the following:

```XML
<?xml version="1.0"?>
<secret sendGridApiKey="<SEND GRID API KEY>"/>
```

When first run the app seeds the database with a default user:

* `Email: admin@admin.com`
* `Password: admin`

After initializing a new admin should be created and this user deleted.
