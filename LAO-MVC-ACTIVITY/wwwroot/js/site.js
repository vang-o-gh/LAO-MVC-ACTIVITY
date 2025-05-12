function deleteTodo(i)
{
  $.ajax({
    url: 'Home/Delete',
    type: 'POST',
    data: {
      id: i
    },
    success: function() {
      window.location.reload();
    }
  });
}

function populateForm(i) 
{
  $.ajax({
      url: 'Home/PopulateForm',
      type: 'GET',
      data: { 
        id: i 
      },
      dataType: 'json',
      success: function(response) {
          $("#Todo_Name").val(response.name);
          $("#Todo_Category").val(response.category);
          $("#Todo_Priority").val(response.priority); 
          $("#Todo_Id").val(response.id);
          $("#form-button").val("Update");
          $("#form-action").attr("action", "/Home/Update");
      }
  });
}

function filterByCategory() {
  document.getElementById("priorityDropdown").value = "";
  const selected = document.getElementById("categoryDropdown").value.toLowerCase();
  const rows = document.querySelectorAll("tbody tr");

  rows.forEach(row => {
      const category = row.cells[2].textContent.toLowerCase();
      if (!selected || category === selected) {
          row.style.display = "";
      } else {
          row.style.display = "none";
      }
  });
}

function filterByPriority() {
  document.getElementById("categoryDropdown").value = "";
  const categoryFilter = document.getElementById("categoryDropdown").value.toLowerCase();
  const priorityFilter = document.getElementById("priorityDropdown").value.toLowerCase();
  const rows = document.querySelectorAll("tbody tr");

  rows.forEach(row => {
      const category = row.children[2].innerText.toLowerCase(); // Category column
      const priority = row.children[3].innerText.toLowerCase(); // Priority column

      const categoryMatch = !categoryFilter || category === categoryFilter;
      const priorityMatch = !priorityFilter || priority === priorityFilter;

      row.style.display = (categoryMatch && priorityMatch) ? "" : "none";
  });
}

function toggleComplete(id, isChecked) {
  const row = document.getElementById(`task-row-${id}`);
    if (isChecked) {
        row.classList.add("completed-task");
    } else {
        row.classList.remove("completed-task");
    }
  fetch(`/Home/ToggleComplete?id=${id}&isChecked=${isChecked}`, {
      method: 'POST'
  }).then(() => location.reload());
}