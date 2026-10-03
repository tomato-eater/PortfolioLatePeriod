// mainパッケージのmain関数がプログラムの開始地点
package main

//ライブラリ宣言のようなもの
import (
	"encoding/json"
	"fmt"
	"log"
	"net/http"
)

type GreetReq struct {
	Name string `json:"name"`
}

type GreetRes struct {
	Message string `json:"message"`
}

// プログラムが開始したときに最初に呼び出される
func main() {
	log.Printf("実行中★")

	//http.HandleFunc どのURLにどんな処理をさせるか
	http.HandleFunc("/", func(w http.ResponseWriter, r *http.Request) {
		fmt.Fprintln(w, "Hello World")
	})

	http.HandleFunc("/json", func(w http.ResponseWriter, r *http.Request) {
		res := GreetRes{Message: "Hello Gopher."}
		w.Header().Set("Content-Type", "application/json")
		if err := json.NewEncoder(w).Encode(res); err != nil {
			http.Error(w, err.Error(), http.StatusInternalServerError)
		}
	})

	addr := ":8080"
	log.Printf("サーバー起動:http://localhost%s", addr)

	if err := http.ListenAndServe(addr, nil); err != nil {
		log.Fatal(err)
	}

}
