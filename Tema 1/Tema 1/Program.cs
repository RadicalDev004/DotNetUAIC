
using Tema_1;
using System.Linq;
using Task = Tema_1.Task;

//SIMPLE TEMA
Task t1 = new("T1", false, DateTime.Now);
Task t2 = new("T2", false, DateTime.Now);

Project p1 = new("Proj", new() { t1 });
Project p2 = p1 with { Tasks = new() { t2 } };

List<Task> AllTasks = new() { t1, t2 };
Manager m = new("Test","test@test.test","team1");

while (true)
{
    Console.Write("Introduceți task ul: ");
    string? val = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(val))
        break;
    
    AllTasks = AllTasks.Select(t => t.Title == val ? t with { IsCompleted = true} : t).ToList();
    
}
Console.WriteLine(string.Join(',', AllTasks.Select(t => t.Title + " " + t.IsCompleted)));

void SwitchType(object o)
{
    switch (o)
    {
        case Task t:
            Console.WriteLine(t.Title  + " " + t.IsCompleted);
            break;
        case Project p:
            Console.WriteLine(p.Name + " " + p.Tasks.Count);
            break;
        default:
            Console.WriteLine("unknown");
            break;
    }
}


//EX 1
Course c = new(".net", 4);
Course c1 = new("test", 2);
Course c2 = new("ml", 6);
Student s = new(1, "Bogdan", 21, new());

Student s2 = s with { Courses = new() { c } };

//EX 2
Instructor i = new Instructor { Name = "i1", Department = "test", Email = "i@i.com"};
Console.WriteLine(i.Name + " " + i.Department + " " + i.Email);

//EX 4
void Ex4(object o)
{
    switch (o)
    {
        case Student stud:
            Console.WriteLine($"{stud.Name} {stud.Courses.Count}");
            break;

        case Course crs:
            Console.WriteLine($"{crs.Title} {crs.Credits}");
            break;

        default:
            Console.WriteLine("unknown type");
            break;
    }
}


//EX5
Func<Course, bool> hasMoreThanFourCredits = static s =>
    s.Credits >= 3;

var sortedCourses = new List<Course>() {c, c1, c2}.Where(hasMoreThanFourCredits);


//EX 3
List<Student> students = new();

while (true)
{
    Console.Write("Introduceți studentul: ");
    string? val = Console.ReadLine();
    var elems = val.Split(' '); 

    if (string.IsNullOrWhiteSpace(val))
        break;

    students.Add(new(int.Parse(elems[0]), elems[1], int.Parse(elems[2]), new()));
}

Console.WriteLine("\nLista completă de studenți:");
foreach (var str in students)
    Console.WriteLine($"- {str.Name} {str.Age}");