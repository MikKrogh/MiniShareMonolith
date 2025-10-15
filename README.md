<h1>TODO:</h1>
delete images when post is deleted*
user deleted*
report posts*
contact support*
Map*
CloudflarePresign could use a cleanup, the class itself should not use count or int, but be more "usuable", something like expose a method that takes a func<x,z,y,string> to create the key, removing the need to a unique method for thumbnail

<h4>
  across the board
</h4>
  <li>
    more metrics and find a better logging structure/pattern across the backend, so they all follow a similar setup
  </li>
  <li>
    improve firebase signup setup, verification email always ends up in spam, and the text in the mail is unfriendly
  </li>

<h4>
  PostsModule
</h4>
  <li>
    PostsRepository could use cleanup, and some benchmarking on the current dynamic filter/search/ orderby -solution, to ensure its nots shite
  </li>
  <li>
    fix this warnning from pagination route **The query uses a row limiting operator ('Skip'/'Take') without an 'OrderBy' operator. This may lead to unpredictable results. If the 'Distinct' operator is used after 'OrderBy', then make sure to use the 'OrderBy' operator after 'Distinct' as the ordering would otherwise get erased.**
  </li>
  <li>
    perform file type check on imageupload, check the file metada value. look into ways of doing this, and other practices to ensure malicious files are not saved.
  </li>
<h4>
  UserModule
</h4>
<li>
  move firebase signup away from the frontend, and into a backend, to ensure that the displayname property is a unique thing
</li>
<li>
  introduce gracefull cleanup flow incase a step fails in the account creation process. Case: user is created in firebase, but the backend fails to introduce the event across backend modules/services making the user stuck in a state where they can login, but are unable to performs actions.
</li>

<h4>
  Frontend
</h4>
<li>
  better error handling for image upload, currently the user has no way of knowing how to handle image upload errors. also we need a frontend warning on filesize limits
</li>
<li>
  admin mode if user is creator on post *filter by created by me settings
</li>
<li>
  contact dev  
</li>
<li>
  delete user  
</li>
<li>
  privacy policy
</li>


<h4>
  YARP PROXY
</h4>
<li>
  stop it from exposing swagger 
</li>


net stop winnat
net start winnat


