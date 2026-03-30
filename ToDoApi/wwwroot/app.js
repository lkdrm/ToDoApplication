console.log("Hello from TypeScript!");
const taskInput = document.getElementById('taskTitle');
const taskDescription = document.getElementById('taskDescription');
const addTaskBtn = document.getElementById('addTaskBtn');
const taskList = document.getElementById('taskList');
await loadTasks();
addTaskBtn.addEventListener('click', async () => {
    const text = taskInput.value;
    if (text.trim() !== "") {
        const newTask = {
            title: text,
            description: taskDescription.value,
            dateTime: new Date().toISOString()
        };
        await fetch('/tasks', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(newTask)
        });
        await loadTasks();
        taskInput.value = "";
        console.log(`Add new task ${taskInput.value}`);
    }
});
async function loadTasks() {
    const response = await fetch('/tasks');
    const tasks = await response.json();
    taskList.innerHTML = '';
    tasks.forEach((task) => {
        const li = document.createElement('li');
        li.textContent = `${task.title} (${task.createdDate}) ${task.description}`;
        const buttonDelete = document.createElement('button');
        buttonDelete.textContent = "x";
        const buttonCompleted = document.createElement('button');
        buttonCompleted.textContent = "✓";
        if (task.isCompleted) {
            li.style.textDecoration = "line-through";
        }
        buttonDelete.addEventListener('click', async () => {
            const valueToDelete = task.id;
            await fetch(`/tasks/${valueToDelete}`, {
                method: 'DELETE'
            });
            await loadTasks();
        });
        buttonCompleted.addEventListener('click', async () => {
            const value = task.id;
            task.isCompleted = !task.isCompleted;
            await fetch(`/tasks/${value}`, {
                method: `PUT`,
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(task)
            });
            await loadTasks();
        });
        li.appendChild(buttonCompleted);
        li.appendChild(buttonDelete);
        taskList.appendChild(li);
    });
}
export {};
//# sourceMappingURL=app.js.map