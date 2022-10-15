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

3rd commit "Implementation"
This commit was done over the course of a few quick sessions writing up tests, and then writing out functionaly. Some major notes
-I got rid of the Unit of work pattern, since my database is only storing a small collection of data. The in memeory database is still useful to avoid having to reparse the csv file over and over
-I used a combination of Csv helper and the Slyvan csv formatter package. The Sylvan package is purely to allow me to post csv data via swagger, . The Csv helper classes are used to deal with parsing either the intital data or proders, mappers are also in place to map the csv to propites
-Csv helper will throw exceptios if headers or missing, or for invalid data(IE the enum doesnt exit). I added a tiny bit of middle in start(hope to move it into its one class, and just it in).
-I tried to implent as much async await as possible, I added a package to some of the projects(System.Linq.Async.Queryable)to allow me to work with more async(mostly enumberables).
-On another note, rather than the app working of the file system directory, I installed system.Io.Abstraction, allowing me to add a File system into the DI, and to mock it a lot easier for unit tests.
-For unit tests, I am using the same test stack as I used for a lot with projects in my last job. Xunit, Moq for mocking, FLuentassertions to perform assertions(more of a preference here, the sytnax is quite readable I found),and Autobogus for generating fake data.
-As for database access, I still wanted to keep it nice and layered(generaly easier to maintain I find), as such the db context isnt used directly bar a service, purely meant for acessing the context
-Using actual csv files for the unit tests isnt a great soultion, the current setup depends on it getting a streamreader, which the libray then uses to prase(this is recived for the body). On my list to refactor this to be a bit cleaner
-Lastly, the deal system and checkout return are not final. I want to clean that up by applying some form of pattern(just a way to avoid bloating the method with new rules, bit of a stub at the minute but the logic is sound, just need a better way to implenet it).


