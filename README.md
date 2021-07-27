# DList.top C# client
Official [dlist.top](https://dlist.top) gateway client for C#.


## Installation

`dotnet add package DlistTop`

## Setup

To get your token please refer to the [DList.top documentation](https://github.com/dlist-top/docs/wiki/Getting-started).


## Usage

```cs
var client = new DlistClient(token);
            
client.OnReady += (sender, args) =>
{
    Console.WriteLine($"ready! connected to {args.Data.Name}");
};

client.OnVote += (sender, args) =>
{
    Console.WriteLine($"{args.Data.AuthorID} voted!");
};

client.OnRate += (sender, args) =>
{
    Console.WriteLine($"{args.Data.AuthorID} rated - {args.Data.Rating} stars!");
};

client.Connect().Wait(); // or await client.Connect();
```

Full example can be found in the *Example* folder.
