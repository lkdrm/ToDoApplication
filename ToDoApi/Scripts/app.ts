console.log("Hello from TypeScript!");

const taskInput = document.getElementById('taskTitle') as HTMLInputElement;
const addTaskBtn = document.getElementById('addTaskBtn') as HTMLButtonElement;
const taskList = document.getElementById('taskList') as HTMLUListElement;

addTaskBtn.addEventListener('click', async () => {
    const text = taskInput.value;

    if (text.trim() !== "")
    {
        const newTask = {
            title: text,
            description: "",
            dateTime: new Date().toISOString()
        };

        await fetch('/tasks',
            {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(newTask)
        });

        const li = document.createElement('li');
        li.textContent = text;
        taskList.appendChild(li);
        taskInput.value = "";

        console.log(`Add new task ${taskInput.value}`);
    }
});