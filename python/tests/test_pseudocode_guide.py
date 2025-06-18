from playground.pseudocode_guide import fetch_data

def test_fetch_data():
    url = "https://jsonplaceholder.typicode.com/posts/1"
    mock_json = {
        "userId": 1,
        "id": 1,
        "title": "sunt aut facere repellat provident occaecati excepturi optio reprehenderit",
        "body": (
            "quia et suscipit\n"
            "suscipit recusandae consequuntur expedita et cum\n"
            "reprehenderit molestiae ut ut quas totam\n"
            "nostrum rerum est autem sunt rem eveniet architecto"
        ),
    }

    data = fetch_data(url)
    assert data["id"] == mock_json["id"]
    assert data["title"] == mock_json["title"]
