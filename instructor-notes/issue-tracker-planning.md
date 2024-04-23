# Issue Tracker

We need a service that allows our users to submit issues for specific software. 

In the future, we'll allow these issues to be used by the help desk people to resolve the issues, but for sprint 1, let's just make it so we 
can create them.



## Adding Issues

Operation: "Add an Issue"
Operands:
    - Software?
    - Description of the problem
    - Who is creating this issue? -- Who is almost always from the Authorization header in the request.

```json
{
    "software": "Excel",
    "description": "I want clippy back!"
}
```

If you did it right, you'll get back something like:

Also include who to contact if it is an urgent matter.


```json
{
    "id": "some-unique-id",
    "software": "Excel",
    "description": "Broke!",
    "createdOn": "2024-timestamp....",
    "status": "Created",
    "support": {
        "name": "Joe",
        "email": "joe@company.com",
        "phone": "555-1212"
    }
}

```

"During business hours (when we are open) it is always Joe."
"We are going to move to a system where that job is rotated amongst people"
"Outside of business hours, it is our contracted company, Support Pros, and you should give them that email and phone number"

Cohesion ("Temporal Cohesion")
