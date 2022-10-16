## Greg Sheerin Prepay power tech assignment

##### A quick note to the reader before hand
I have been using spell checkers on anything I write, but if you spot any grammar/spelling mistakes/typos I do apologise, I am dylexic(fairly sure I let Oscar know) but just incase I forgot I figured it best to mention it here.

## General approach

After reading the spec I had a rough Idea for a good design for this project. 
1. For a scalable project I immediately think of some api based solution. Rather than tieing front end to backend, the backend can handle any requests/business logic(in its own layer) while being completely agnostic of anything else.
The general idea is that the api can be built in such a way that several clients could interact with the same system and work with the business rules in whatever way they see fit(display it, audit log it, ect). Also with the way I plan to structure/implement this. I am hoping the api would be robust enough to handle many requests at a time(I lack a profiler for this)
2.Each part of the application should be laid, as In the API handles the request, a domain layer handles any business logic, and a data layer handles any data related things(see data handling below).
3.Generaly speaking, before I do any "proper" implementation, I will write a tests to cover what I am expecting, I find that even If I don't cover everything initial, it servers to highlight flaws in the implementation as I work, rather than a lump of debugging at the end
4. I wanted to try and use as much async await style in the application, generally speaking, unless the operation must be sync for business reasons, it's better to run async in my mind, as it makes better use of the hardware
That and in .net core, it isn't terribly difficult to implement. I ended up using a package for this called(System.linq.async.queryable) to allow me to convert lists to async emuerators, thus allowing more async work to be done per request.
5. In terms of letting the reading know what I am doing, I aim to give a general thought process to approach in here, and more particular details in the comments, if you have any questions about anything feel free to email me @gregorysheerin@gmail.com

## Assumptions / Statements on impletations
1. I am assuming that once data is set in the api, there is no need to change it(more on this on data section)
2. I am also assuming that bonus items on the orders(extra aubergines for example) do not affect any other bonus(avoid a loop of getting aubergines for free, then getting a discount on them ect).
3. The csv parser isn't a great solution(well the way I get the list in the controller, I would like the stream to be out of the controller). In an ideal world the api would be dealing json or xml(CSv would then be treated like a large file upload, using Ifile instead, and handled thusly)
4. The api returns json at the moment,I was going to change it to a csv, but the assignment did not specify it, and again the idea is that anything application and get the info and do with it what it needs
5. The current system for handing the deals wasn't great. In hindsight I could have spent a bit more time on that, but the unit tests validate the logic at this point, so a refactor would be a lot easier to undertake(see service belows).

### Data handling
I chose to use entity frameworks in the memory database to act as a store for the product details on startup. This isn't the best database implementation around, but my goal was to get something that was easy to run(does not need a database) but that
could be extended out into a proper database implementation. That and entity framework allows for a lot of cleaning of a structure application wise I find(IE the data layer is the only thing working with the mock database, and same with an actual sql database).
While it isn't used here, another reason I wanted to use the entity framework is because it opens the door to a very clean and robust CICD process with migrations. The idea would be each change to the database(initial setup, seeding, set an index ect) would be created as a migration
via entity framework, then on the deploy any target machine will pick back up wherever it last was updated(EF usings a formating system to detect this). While doing this mean that the actual
migrations would be immutable, it does mean to have a running track record of database changes that are easy to apply across multiple environments(more on this in the ending notes).

### Service implementation
Note on the deals here, I mentioned before it wasnt ideal. Rather than just leave it in the air, I figured id sum up what I would envisage for this
1. A collection of deals would be stored in the database, with flags saying active or not, these could be pulled out by entity framework easily enough
2. Rather than using Enums for products, IDs would be implemented, and then stored in the database(and thus replace the enum checks with ids).
3. A collection of deals would be parsed(this would need a lot more fleshing out in terms of requirements, for example as the deals static, or is it a case we need to agree on some standard format, parse it and then apply it. There are many ways to approach this)
4. Once an order is received, iterate through the deals and apply them. Each deal should either be its own class, or contain something parseable(so we can then translate that into some form of maths to apply the deal, IE buy X of Y and get 1 Z free)
5. I did make sure however that what you buy and what you get are seperate. In the response I return the actual order and the items you got separately(as such consumers can keep track of what was sent and what was received, mutating the original order didn't seem like a great idea business logic wise)

### Console app
The console app was designed to be a very simple and bare bones test harness, to just perform the call and demo the details you get back. During development I used postman locally, and I do have swagger set up if you would like to use that.
The main assignment is the API(the majority of the effort that is to say), so I didn't spend much time creating the console, as it is just a quick way to call it pre set up. Hardcoding urls is bad, in an actual app I'd use a config class to pull values out depending on the environment, or just them injected via octopus or something akin to it.

### Unit Testing
I went with a TDD approach when implementing the tests, as I had an idea of what the system would do, regardless of the implementation.
For example, once I checkout, I know I'll get an order receipt(this changed during implantation from checkout), i know i'll get a type back. Since I abstract all the services behind an interface, I can easily mock returns via moq.
This let me write a few tests for what I expect a thing to do, implement it, and then follow the red green factor pattern until all was solid. Granted I did miss a few cases during my tests.
My testing approach was generally to use the tests to validate my work, even it comes to DI injections, or database setup. If I changed something that caused an issue, I knew fairly quickly. I did scaffold a few tests out at the start, but only a bare
bones structure to save me having to create the folder and files later.

### Running the application
The api spins up on localhost with the port of 5001, launching it under the "prepay.vegetable.api" will get it going. At the point you can either call the api via your preferred tool,
Or right click on prepay.vegetable.consoleworkbench" -> debug -> start new instance. The console app fires off a request, and display the parsed results to the console window.

### Going forward with this
As I hinted at in the database section. I would do several things going forward to improve and deploy the application.
1.Perfrom refactors to services, data models, ect.
2. Apply some form of test coverage analytic tool(via cicd maybe see below)
3. Add some form of integration testing(We used fitnesse in eurofins but I believe there are far better ones available, but just for example).
4. Add configurations for development vs deployment(or add injection tokens for something like octopus, depends on the stack but generally to improve cicd)
5.1.Create a CICD pipeline to accompany this. One build would trigger on a pull request(this would build+test,maybe even block based on code analytics from sonar qube for example)
5.2.The second build would do the same steps, and then push it out to wherever it went, for example octopus to send it off to a remote machine.
Given time constraints I wouldn't of been able to get the above going, but when creating the app it is something I kept in mind, as once this kind of pipeline is in place it allows for fast, high quality commits to be done,
and thus reducing the time spent tracking down issues.

### Closing notes
Thanks for reading though the massive wall of text(and apologies but I do like to have notes on my thought process as a part of any assignment), below are the notes I kept at as I committing code/working, do keep in mind this are not proof read, and just are there are a track record fo what I was doing what I was working on this assignment.

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



