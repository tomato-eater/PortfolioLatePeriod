package main

import (
	"encoding/json"
	"log"
	"net/http"
	"sync"
)



type Item struct {
	ID	 int	 `json:"id"`
	Name string	 `json:"name"`
}

// データ保管場所
var(
	mu sync.Mutex
	items = []Item{}
	nextID = 1
)



func main() {
	http.HandleFunc("/items", itemsHandler)

	addr := ":8080"
	log.Printf("Starting server on http://localhost%s", addr)

	if err := http.ListenAndServe(addr, nil); err != nil {
		log.Fatalf("Failed to start server: %v", err)
	}

}
// itemsHandler handles requests to the /items endpoint.
func itemsHandler(w http.ResponseWriter, r *http.Request) {
	switch r.Method {
		case http.MethodGet:
			handleGet(w, r)
		case http.MethodPost:
			handlePost(w, r)

		default:	// Handle unsupported methods
			http.Error(w, "Method not allowed", http.StatusMethodNotAllowed)
	}
}

func handleGet(w http.ResponseWriter, r *http.Request) {
	mu.Lock()
	defer mu.Unlock()

	w.Header().Set("Content-Type", "application/json")
	if err := json.NewEncoder(w).Encode(items); err != nil {
		http.Error(w, "Failed to encode items: " + err.Error(), http.StatusInternalServerError)
	}

}


func handlePost(w http.ResponseWriter, r *http.Request) {
	var in Item
	if err := json.NewDecoder(r.Body).Decode(&in); err != nil {
		http.Error(w, "json decode error: " + err.Error(), http.StatusBadRequest)
		return
	}
	
	if in.Name == "" {
		http.Error(w, "name is required", http.StatusBadRequest)
		return
	}
	if in.Name == " " {
		http.Error(w, "name cannot be empty", http.StatusBadRequest)
		return
	}

	mu.Lock()
	
	in.ID = nextID
	nextID++
	items = append(items, in)

	mu.Unlock()
	
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusCreated) //201 Created
	if err := json.NewEncoder(w).Encode(in); err != nil {
		http.Error(w, "Failed to encode item: " + err.Error(), http.StatusInternalServerError)
		return
	}	
}