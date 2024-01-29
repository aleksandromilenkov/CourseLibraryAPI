#RESTful API

This API is using SQl Lite database.  
I'm using 2 entities, Authors and Courses. Every Author has a list of Courses and every Course has it's author. So I'm using One-To-Many Relationship between the models.  
The API also suports XML format for returning and receiving type.  
You can also send HEAD request and OPTIONS request beside the GET, POST, PUT, PATCH and DELETE.  
You can also Upserting with PUT and PATCH.  
The API has good validation with custom messages. And also using Class-Level validation.  
The API also supports filtering, searching and paging.  
The API is using also Data-Shaping where the consumer can specify which fields from the Model are needed.  
The API is tested with POSTMAN and it is working properly.  
