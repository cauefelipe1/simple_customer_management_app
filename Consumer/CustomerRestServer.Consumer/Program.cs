namespace CustomerRestServer.Consumer;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Inform the amount of customers to be created.");
        
        string userInput = Console.ReadLine();
        int customersAmount = int.Parse(userInput);
        
        var exec = new CustomerManagementRestClient();
            
        await exec.Execute(customersAmount);
    }
}