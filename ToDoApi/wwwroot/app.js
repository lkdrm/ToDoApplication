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
        taskDescription.value = "";
        console.log(`Add new task ${taskInput.value}`);
    }
});
async function loadTasks() {
    const response = await fetch('/tasks');
    const tasks = await response.json();
    taskList.innerHTML = '';
    tasks.forEach((task) => {
        const li = document.createElement('li');
        const titleElement = document.createElement('strong');
        titleElement.textContent = task.title;
        titleElement.classList.add('task-title-text');
        const dateElement = document.createElement('small');
        dateElement.textContent = new Date(task.createdDate).toLocaleDateString();
        dateElement.classList.add('task-date-text');
        const descElement = document.createElement('p');
        descElement.textContent = task.description || "No description";
        descElement.classList.add('task-desc-text');
        const buttonsContainer = document.createElement('div');
        buttonsContainer.classList.add('task-buttons-container');
        const buttonCompleted = document.createElement('button');
        buttonCompleted.textContent = "✓ Ready";
        buttonCompleted.classList.add('task-btn', 'btn-ready');
        const buttonDelete = document.createElement('button');
        buttonDelete.textContent = "x Delete";
        buttonDelete.classList.add('task-btn', 'btn-delete');
        if (task.isCompleted) {
            li.classList.add('task-completed');
            buttonCompleted.disabled = true;
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
        li.appendChild(titleElement);
        li.appendChild(dateElement);
        li.appendChild(descElement);
        buttonsContainer.appendChild(buttonCompleted);
        buttonsContainer.appendChild(buttonDelete);
        li.appendChild(buttonsContainer);
        taskList.appendChild(li);
    });
}
export {};
//# sourceMappingURL=app.js.map