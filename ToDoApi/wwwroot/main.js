"use strict";
async function startApp() {
    console.log("Start server!");
    const taskInput = document.getElementById('taskTitle');
    taskInput.addEventListener('input', validInput);
    const taskDescription = document.getElementById('taskDescription');
    taskDescription.addEventListener('input', validInput);
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
    function validInput() {
        const isTitleEmpty = taskInput.value.trim() === "";
        const isDescriptionEmpty = taskDescription.value.trim() === "";
        addTaskBtn.disabled = isTitleEmpty || isDescriptionEmpty;
        if (isTitleEmpty) {
            taskInput.classList.add('input-error');
        }
        else {
            taskInput.classList.remove('input-error');
        }
        if (isDescriptionEmpty) {
            taskDescription.classList.add('input-error');
        }
        else {
            taskDescription.classList.remove('input-error');
        }
    }
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
            const buttonEdit = document.createElement('button');
            buttonEdit.textContent = "✏️ Edit";
            buttonEdit.classList.add('task-btn', 'btn-edit');
            buttonEdit.addEventListener('click', async () => {
                debugger;
                const newTitle = prompt("Enter new title:", task.title);
                if (newTitle === null) {
                    return;
                }
                else if (newTitle.trim() === '' || newTitle.trim() === "") {
                    alert("Title cannot be empty!");
                    return;
                }
                const newDescription = prompt("Enter new description:", task.description);
                if (newDescription === null) {
                    return;
                }
                else if (newDescription.trim() === '' || newDescription.trim() === "") {
                    console.log("Im here");
                    alert("Description cannot be empty!");
                    return;
                }
                task.title = newTitle;
                task.description = newDescription;
                const value = task.id;
                await fetch(`/tasks/${value}`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(task)
                });
                await loadTasks();
            });
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
            buttonsContainer.appendChild(buttonEdit);
            li.appendChild(buttonsContainer);
            taskList.appendChild(li);
        });
    }
}
startApp();
//# sourceMappingURL=main.js.map