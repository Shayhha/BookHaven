
function showPopup(url, name, author, date, bookId, category, format, ageLimitation, price, stock, salePrice) {
    document.getElementById('popup-image').setAttribute('src', url);
    document.getElementById('popup-title').innerHTML = name;
    document.getElementById('popup-bookId').innerHTML = bookId;
    document.getElementById('popup-author').innerHTML = author;
    document.getElementById('popup-date').innerHTML = date;
    document.getElementById('popup-category').innerHTML = category;
    document.getElementById('popup-format').innerHTML = format;
    document.getElementById('popup-age').innerHTML = ageLimitation;
    document.getElementById('popup-stock').innerHTML = stock;

    if (salePrice != 0 && price != salePrice)
        document.getElementById('popup-price').innerHTML = '<strong>$' + salePrice + '</strong> <strike>$' + price + '</strike>';
    else
        document.getElementById('popup-price').innerHTML = '$' + price;


    document.getElementById('overlay').style.display = 'block';
    document.getElementById('popup').style.display = 'block';

    checkStockAvailability()
}

function closePopup() {
    document.getElementById('overlay').style.display = 'none';
    document.getElementById('popup').style.display = 'none';
}

function checkStockAvailability() {
    var stock = document.getElementById('popup-stock').innerText;
    var notifyButton = document.getElementById('bookNotifyMeButton');
    var bookInfoButton = document.getElementById('bookInfoPageButton');
    if (stock.trim() === '0') {
        notifyButton.style.display = 'block';
        bookInfoButton.style.display = 'none';
    } else {
        notifyButton.style.display = 'none';
        bookInfoButton.style.display = 'block';
    }
}


function filterButtonOnClick() {
    var filterValues = [];
    var searchQueryElement = document.getElementById("searchQueryString");
    var searchQuery = searchQueryElement ? searchQueryElement.innerText : ""; // Assign empty string if searchQueryElement is null
    var filterSelects = document.getElementsByClassName("filter-select");
    var optionsSelected = false; // Flag to check if any option is selected

    for (var i = 0; i < filterSelects.length; i++) {
        var selectedOption = filterSelects[i].value; // Get the value of the selected option
        if (selectedOption !== "") {
            filterValues.push(selectedOption); // Push the selected option's value into the array
            optionsSelected = true;
        }
    }

    if (!optionsSelected) {
        alert("Please select at least one option.");
        return; // Exit function if no option is selected from any filter
    }

    var filterString = filterValues.join(","); // Join the values with commas
    console.log("Selected values: ", filterString);
    window.location.href = '/SearchResults/showfilteredBooks?filterBy=' + filterString + '&searchQuery=' + searchQuery;
}

function clearFilterButtonOnClick() {
    var filterSelects = document.getElementsByClassName("filter-select");
    for (var i = 0; i < filterSelects.length; i++) {
        filterSelects[i].value = "";
    }
    window.location.href = '/UserHome/showUserHome';
}

function handleSearchButtonClick() {
    var searchQuery = document.getElementsByName("searchQuery")[0].value;
    window.location.href = '/SearchResults/showSearchResults?searchQuery=' + encodeURIComponent(searchQuery);
}

function handleCategoryButtonClick(element) {
    var searchQuery = element.textContent.trim();
    window.location.href = '/SearchResults/showCategoryResults?searchQuery=' + encodeURIComponent(searchQuery);
}



function addBookToCart() {
    var bookId = document.getElementById("popup-bookId").textContent;
    var quantity = document.getElementById("quantityInput").value;
    // Make an AJAX request to call the addBookToCart method
    fetch(`/CartItem/addBookToCart?bookId=${bookId}&quantity=${quantity}`)
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                alert('Book added to cart successfully!');
            } else {
                if (data.errorMessage === "")
                    alert('Failed to add book to cart. Please try again later.');
                else
                    alert(data.errorMessage);
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('An error occurred while adding the book to cart. Please try again later.');
        });
}



function goToBookInfoPage() {
    var bookId = document.getElementById("popup-bookId").textContent;
    var editUrl = "/Book/showBookInfoView?bookId=" + bookId;
    window.location.href = editUrl;
}

function editBook() {
    var bookId = document.getElementById("popup-bookId").textContent;
    var editUrl = "/SearchResults/showBookDetails?bookId=" + bookId;
    window.location.href = editUrl;
}

function restockBook() {
    var bookId = document.getElementById("popup-bookId").innerText;
    var restockAmount = document.getElementById("restockAmount").value; // Use value instead of textContent
    var editUrl = "/Book/restockBook?bookId=" + bookId + "&restockAmount=" + restockAmount;
    window.location.href = editUrl;
}

function putBookOnSale() {
    var bookId = document.getElementById("popup-bookId").innerText;
    var salePrice = document.getElementById("salePrice").value; // Use value instead of textContent
    var editUrl = "/Book/putBookOnSale?bookId=" + bookId + "&salePrice=" + salePrice;
    window.location.href = editUrl;
}

function removeBookFromSale() {
    var bookId = document.getElementById("popup-bookId").textContent;
    var editUrl = "/Book/removeBookFromSale?bookId=" + bookId;
    window.location.href = editUrl;
}

function buyBook() {
    var bookId = document.getElementById("popup-bookId").textContent;
    var quantity = document.getElementById("quantityInput").value;
    var editUrl = "/Payment/processPayment?bookId=" + bookId + "&quantity=" + quantity;
    window.location.href = editUrl;
}

function deleteBook() {
    var bookId = document.getElementById("popup-bookId").textContent;
    var editUrl = "/Book/deleteBook?bookId=" + bookId;

    closePopup();

    window.location.href = editUrl;
}

function deleteBook2() {
    var bookId = document.getElementById("popup-bookId").textContent;
    var editUrl = "/Book/deleteBook?bookId=" + bookId;
    window.location.href = editUrl;
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

function showNotifyMessage() {
    alert("You will be notified once this book is restocked on our shelves. Sorry for the inconvenience.");
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




// Clearing the default user's cart when he exits our app or redirects to a different url (outside of our app).
function handleBeforeUnload(e) {
    window.addEventListener('beforeunload', function (e) {
        // Execute your function here
        fetch(`/CartItem/clearDefaultUserCart`)
            .then(response => response.json());
    });
}

handleBeforeUnload();