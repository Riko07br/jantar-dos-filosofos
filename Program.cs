namespace JantarDosFilosofos; 
internal class Program {

    static Fork[] forks;
    static int[] timesEaten;

    enum State {
        THINKING = 0,  // philosopher is THINKING
        HUNGRY = 1,    // philosopher is trying to get forks
        EATING = 2,    // philosopher is EATING
    };    

    static void Main(string[] args) {
        string[] philosopherNames = { "Humberto", "Roberto", "Alberto", "Godoberto", "Felisberto" };        
        timesEaten = new int[5];

        forks = new Fork[5];
        for (int i = 0; i < forks.Length; i++) {
            forks[i] = new Fork();
        }

        Console.WriteLine("Filosofos definidos");
        Console.WriteLine(philosopherNames.ToString());
        Console.WriteLine("Pressione enter para continuar...");

        Console.ReadLine();
        for (int i = 0; i < philosopherNames.Length; i++) {
            Thread t = new Thread(new ThreadStart(() => ThreadProc(i, philosopherNames[i])));
            t.Start();
            Thread.Sleep(10);
            //Console.WriteLine(philosopherNames[i]);
        }

        //ocupy main thread
        while (true) {             
            Thread.Sleep(100);
        }

    }

    public static void ThreadProc(int id, string philosopherName) {

        Philosopher philosopher = new Philosopher(id, philosopherName);
        Console.WriteLine(philosopher.name + " sentou");
        while (true) {
            Thread.Sleep(100);  //time in ms
            philosopher.Act();
        }
    }

    class Philosopher {
        public int id;
        public string name;
        public State state = State.HUNGRY;

        int LeftId { get => id == 0 ? forks.Length - 1 : id - 1; }

        public Philosopher(int id, string name) {
            this.id = id;
            this.name = name;
        }

        public void Act() {
            //the last one tries to get other fork to avoid deadlock
            int firstSideToCheck = id == forks.Length - 1 ? LeftId : id;
            int secondSideToCheck = id != forks.Length - 1 ? LeftId : id;

            switch (state) {
                case State.THINKING:
                    Console.WriteLine(name + " esta pensando...");
                    Thread.Sleep(2000);
                    state = State.HUNGRY;

                    break;
                case State.HUNGRY:
                    Console.WriteLine(name + " esta com fome, vai tentar pegar garfos");
                    // try to get right fork only if he wasnt the last one to use it
                    while (forks[firstSideToCheck].inUse) {
                        Thread.Sleep(5);
                    }

                    forks[firstSideToCheck].lastUser = this;
                    forks[firstSideToCheck].inUse = true;
                    Console.WriteLine(name + " Pegou um garfo");

                    // try to get left fork only if he wasnt the last one to use it
                    while (forks[secondSideToCheck].inUse) {
                        Thread.Sleep(5);
                    }

                    forks[secondSideToCheck].lastUser = this;
                    forks[secondSideToCheck].inUse = true;
                    Console.WriteLine(name + " Pegou outro garfo e vai comer");
                    state = State.EATING;

                    break;
                case State.EATING:
                    Thread.Sleep(2000);

                    forks[firstSideToCheck].lastUser = null;
                    forks[firstSideToCheck].inUse = false;
                    forks[secondSideToCheck].lastUser = null;
                    forks[secondSideToCheck].inUse = false;
                    Console.WriteLine(name + " comeu e soltou os garfos");
                    timesEaten[id]++;
                    Console.WriteLine(string.Join("-", timesEaten));                    
                    state = State.THINKING;

                    break;
                default:
                    break;
            }
        }
    }

    class Fork {
        public Philosopher? lastUser = null;
        public bool inUse = false;
    }

}