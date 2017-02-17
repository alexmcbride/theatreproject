LOCAL THEATRE COMPANY PROJECT
-----------------------------

For the site to run the following file needs to be placed in the root site directory (alongside Web.config):

Secret.config

This file should contain the following:

<?xml version="1.0"?>
<secret sendGridApiKey="<SEND GRID API KEY>"/>

When first run the app seeds the database with a default user:

Email: admin@admin.com
Password: admin

After initializing a new admin should be created and this user deleted.

Thanks,

Alex!