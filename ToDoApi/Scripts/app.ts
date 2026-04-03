async function startApp() {
    console.log("Start server!");

    const taskInput = document.getElementById('taskTitle') as HTMLInputElement;
    taskInput.addEventListener('input', validInput);
    const taskDescription = document.getElementById('taskDescription') as HTMLTextAreaElement;
    taskDescription.addEventListener('input', validInput);
    const addTaskBtn = document.getElementById('addTaskBtn') as HTMLButtonElement;
    const taskList = document.getElementById('taskList') as HTMLUListElement;

    let currentFilter: 'all' | 'active' | 'completed' = 'all';

    const filterAllBtn = document.getElementById('filterAll') as HTMLButtonElement;
    const filterActiveBtn = document.getElementById('filterActive') as HTMLButtonElement;
    const filterCompletedBtn = document.getElementById('filterCompleted') as HTMLButtonElement;

    await loadTasks();

    function setFilter(filterName: 'all' | 'active' | 'completed') {
        currentFilter = filterName;

        filterAllBtn.classList.remove('active-filter');
        filterActiveBtn.classList.remove('active-filter');
        filterCompletedBtn.classList.remove('active-filter');

        if (filterName === 'all') {
            filterAllBtn.classList.add('active-filter');
        }

        if (filterName === 'active') {
            filterActiveBtn.classList.add('active-filter');
        }

        if (filterName === 'completed') {
            filterCompletedBtn.classList.add('active-filter');
        }

        loadTasks();
    }

    filterAllBtn.addEventListener('click', () => setFilter('all'));
    filterActiveBtn.addEventListener('click', () => setFilter('active'));
    filterCompletedBtn.addEventListener('click', () => setFilter('completed'));

    addTaskBtn.addEventListener('click', async () => {
        const text = taskInput.value;

        if (text.trim() !== "") {
            const newTask =
            {
                title: text,
                description: taskDescription.value,
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

            await loadTasks();
            taskInput.value = "";
            taskDescription.value = "";
            console.log(`Add new task ${taskInput.value}`);
            validInput();
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
        let tasks = await response.json();
        tasks.sort((a: any, b: any) => {
            const timeA = new Date(a.createdDate || a.dateTime).getTime();
            const timeB = new Date(b.createdDate || b.dateTime).getTime();

            if (!isNaN(timeA) && !isNaN(timeB) && timeA !== timeB) {
                return timeB - timeA;
            }

            return b.id - a.id;
        });

        if (currentFilter === 'active') {
            tasks = tasks.filter((task: any) => !task.isCompleted);
        }
        else if (currentFilter === 'completed') {
            tasks = tasks.filter((task: any) => task.isCompleted);
        }

        taskList.innerHTML = '';

        tasks.forEach((task: any) => {
            const li = document.createElement('li');
            li.classList.add('task-animate-in');

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

            let isEditing = false;

            buttonEdit.addEventListener('click', async () => {
                if (!isEditing) {
                    isEditing = true;

                    const editTitleInput = document.createElement('input');
                    editTitleInput.type = 'text';
                    editTitleInput.value = task.title;
                    editTitleInput.classList.add('input-field', 'title-input', 'edit-inline-input');

                    const editDescInput = document.createElement('textarea');
                    editDescInput.value = task.description;
                    editDescInput.classList.add('input-field', 'desc-input', 'edit-inline-input');

                    li.replaceChild(editTitleInput, titleElement);
                    li.replaceChild(editDescInput, descElement);

                    buttonEdit.textContent = "💾 Save";
                } else {

                    const currentTitleInput = li.querySelector('input') as HTMLInputElement;
                    const currentDescInput = li.querySelector('textarea') as HTMLTextAreaElement;

                    const newTitle = currentTitleInput.value;
                    const newDescription = currentDescInput.value;

                    if (newTitle.trim() === "") {
                        alert("Title cannot be empty!");
                        currentTitleInput.classList.add('input-error');
                        return;
                    }
                    if (newDescription.trim() === "") {
                        alert("Description cannot be empty!");
                        currentDescInput.classList.add('input-error');
                        return;
                    }

                    task.title = newTitle;
                    task.description = newDescription;

                    buttonEdit.textContent = "⏳ Saving...";
                    buttonEdit.disabled = true;

                    const value = task.id;
                    await fetch(`/tasks/${value}`,
                        {
                            method: 'PUT',
                            headers: {
                                'Content-Type': 'application/json'
                            },
                            body: JSON.stringify(task)
                        });
                    await loadTasks();
                }
            });

            if (task.isCompleted) {

                li.classList.add('task-completed');
                buttonCompleted.disabled = true;
            }
            buttonDelete.addEventListener('click', async () => {
                const valueToDelete = task.id;
                await fetch(`/tasks/${valueToDelete}`,
                    {
                        method: 'DELETE'
                    });
                await loadTasks();
            });

            buttonCompleted.addEventListener('click', async () => {
                const value = task.id;
                task.isCompleted = !task.isCompleted;
                await fetch(`/tasks/${value}`,
                    {
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