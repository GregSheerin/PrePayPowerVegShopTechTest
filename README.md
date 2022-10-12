"# PrePayPowerVegShopTechTest" 
I will be using this readme to keep notes are I work on the project, Ill complile them into some more complete toward the end, write a summary, and push the notes toward the bottom.

Inital Commit
Simple Commit to set up the repo with a readme

2nd commit "Scaffolding"
I had a rough idea of how to structure out an api for use in the project, so I wanted to get the rough outline of the project layout before I start writing anything concrete.
The goals were to
-Set up an API layer to handle a request to parse a list
-Set up a domain layer to be used to handle the parsing of a checkout(IE get the price, list of items, apply offers ect)
-Set up a Data base using a in memeory database to serve as a dummy DB for the assigment(more of a strut than anything else, idea is to have store the csv data later)
-Set up a unit of work to handle the data side, this ensures that the data context is uniform thoughout the app(IE in the event its extended with more contexts, and generaly just ensuring concurency)
-Set up the dependacy injection to provide the services/ dependaices for the applcation.
-Set up controllers/services/repos ect with the DI, to ensure the structure of the applcation(IE run it and get swagger to appear, and debug though a call to see if everything is working as expected).
-Set up a unit test project, and roughly set up each test class ready for testing.

Having this done going forward will make imeplenting the logic proper a lot easier, all the code in plate is "boilerplate" and is just there to check if everything is connected as I intended. Parts of it will change little, but going forward
Ill be writing the tests with the expected output(for example I know the CheckoutProducts endpoint will return a checkout), then the code, refactor and test again(TDD, didnt want to write tests for setting it up, but with way the project is layed out it is easier to test refactor test).
Other minor things that were done were to edit some lanuch settings, install some packages, and get a git ingore
