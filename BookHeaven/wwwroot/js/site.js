// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function handleSearchButtonClick() {
    var searchQuery = document.getElementsByName("searchQuery")[0].value;
    window.location.href = '/SearchResults/showSearchResults?searchQuery=' + encodeURIComponent(searchQuery);
}

function deleteAddress() {
    // AJAX request to delete credit card information
    fetch('DeleteAddress', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({}),
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            console.log('Address deleted');
            document.getElementById('addressSpan').innerHTML = '<span></span>';
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}

function deleteCreditCard() {
    // AJAX request to delete credit card information
    fetch('DeleteCreditCard', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({}),
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            console.log('Credit Card deleted');
            document.getElementById('creditCardSpan').innerHTML = '<span></span>';
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}

function confirmDelete() {
    return confirm("Are you sure you want to delete this item?");
}


function editUserInfoButtonClick() {
    window.location.href = '/Profile/showEditProfileView';
}

function userLogout() {
    window.location.href = '/UserHome/userLogout';
}

function showEmailTakenMessage() {
    alert("This email is already taken. Please choose another one.");
}
