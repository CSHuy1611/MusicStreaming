// =====================================================
// SECTION 1: MUSIC PLAYER CORE
// =====================================================

// ===== 1.1 Global Variables =====
let audio = document.getElementById("audio-source");
let playBtnIcon = document.getElementById("icon-play-pause");
let progressBar = document.getElementById("progress-bar");
let currTime = document.getElementById("current-time");
let durTime = document.getElementById("duration-time");
let isCurrentSongCounted = false;
let currentPlayingSongId = null;
let globalQueue = [];
let globalQueueIndex = 0;
let isShuffle = false;
let isRepeat = false;


// ===== 1.2 Play Music From Song List =====
function playMusic(songId, url, title, artist, img, isSongVip) {

    // 1. SỬA LỖI: Dùng đúng tên tham số isSongVip
    if (isSongVip) {
        // currentUserIsVip là biến toàn cục lấy từ View
        if (typeof currentUserIsVip === 'undefined' || currentUserIsVip === false) {

            // Tạm dừng bài hát hiện tại (nếu đang phát bài khác)
            let audioElement = document.getElementById('audio-source');
            if (audioElement) audioElement.pause();

            // Hiển thị Popup mời nâng cấp VIP
            let vipModal = new bootstrap.Modal(document.getElementById('vipUpgradeModal'));
            vipModal.show();

            // Lệnh return này cực kỳ quan trọng: Nó ngắt hàm tại đây, không cho chạy code phát nhạc!
            return;
        }
    }

    currentPlayingSongId = songId;
    isCurrentSongCounted = false;
    console.log("Đang phát bài:", title);

    // Reset lại icon tim trước khi gọi API kiểm tra
    let heartIcon = document.getElementById('player-heart-icon');
    if (heartIcon) {
        heartIcon.className = "fa-regular fa-heart ms-3 text-secondary";
    }

    // Gọi API kiểm tra trạng thái yêu thích
    fetch('/Favorite/CheckStatus?songId=' + songId)
        .then(response => response.json())
        .then(data => {
            let currentHeart = document.getElementById('player-heart-icon');
            if (currentHeart) {
                if (data.isLiked) {
                    currentHeart.className = "fa-solid fa-heart ms-3 text-danger"; // Đã thích
                } else {
                    currentHeart.className = "fa-regular fa-heart ms-3 text-secondary"; // Chưa thích
                }
            }
        })
        .catch(err => console.error("Lỗi khi kiểm tra trạng thái thả tim:", err));

    // Cập nhật UI của Player
    document.getElementById("player-title").innerText = title;
    document.getElementById("player-artist").innerText = artist;
    document.getElementById("player-img").src = img;

    audio.src = url;
    audio.play();
    updatePlayIcon(true);
}


// ===== 1.3 Toggle Play / Pause =====
function togglePlay() {
    if (audio.paused) {
        audio.play();
        updatePlayIcon(true);
    } else {
        audio.pause();
        updatePlayIcon(false);
    }
}


// ===== 1.4 Update Play Icon =====
function updatePlayIcon(isPlaying) {
    if (isPlaying) {
        playBtnIcon.classList.remove("fa-circle-play");
        playBtnIcon.classList.add("fa-circle-pause");
    } else {
        playBtnIcon.classList.remove("fa-circle-pause");
        playBtnIcon.classList.add("fa-circle-play");
    }
}


// ===== 1.5 Update Progress Bar =====
audio.ontimeupdate = function () {
    if (audio.duration) {
        const progressPercent = (audio.currentTime / audio.duration) * 100;
        progressBar.value = progressPercent;

        currTime.innerText = formatTime(audio.currentTime);
        durTime.innerText = formatTime(audio.duration);

        updateSliderBackground(progressBar, progressPercent);

        // THÊM LOGIC TĂNG VIEW SAU KHI NGHE 10 GIÂY
        if (!isCurrentSongCounted && audio.currentTime >= 10 && currentPlayingSongId) {

            isCurrentSongCounted = true; // Khóa ngay lập tức để không bị gọi API nhiều lần

            fetch('/Song/IncrementListen?id=' + currentPlayingSongId, {
                method: 'POST'
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        console.log("Đã cộng 1 lượt nghe cho bài hát ID:", currentPlayingSongId);
                    }
                })
                .catch(err => console.error("Lỗi khi tăng lượt nghe:", err));
        }
    }
};


// ===== 1.6 Seek Song =====
function seekSong() {
    const seekTime = (audio.duration / 100) * progressBar.value;
    audio.currentTime = seekTime;

    updateSliderBackground(progressBar, progressBar.value);
}


// ===== 1.7 Set Volume =====
function setVolume() {
    const volumeBar = document.getElementById("volume-bar");
    audio.volume = volumeBar.value;

    const volumePercent = volumeBar.value * 100;
    updateSliderBackground(volumeBar, volumePercent);
}


// ===== 1.8 Helper: Format Time =====
function formatTime(seconds) {
    const min = Math.floor(seconds / 60);
    const sec = Math.floor(seconds % 60);
    return `${min}:${sec < 10 ? '0' + sec : sec}`;
}


// ===== 1.9 Helper: Update Slider Background =====
function updateSliderBackground(rangeInput, percent) {
    rangeInput.style.background =
        `linear-gradient(to right, #1db954 ${percent}%, #4d4d4d ${percent}%)`;
}



// =====================================================
// SECTION 2: PLAYLIST MANAGEMENT
// =====================================================

let addToPlaylistModal;


// ===== 2.1 Open Add To Playlist Modal =====
function openAddToPlaylistModal(songId) {

    document.getElementById('hiddenSongIdToAdd').value = songId;

    fetch('/Playlist/GetUserPlaylists')
        .then(response => response.json())
        .then(data => {

            let container = document.getElementById('userPlaylistsContainer');
            container.innerHTML = '';

            if (data.length === 0) {
                container.innerHTML =
                    '<p class="text-secondary small text-center mb-0">Bạn chưa có Playlist nào.</p>';
            } else {
                data.forEach(playlist => {

                    let btn = document.createElement('button');
                    btn.className =
                        'btn text-white text-start w-100 p-2 rounded';
                    btn.style.backgroundColor = '#3e3e3e';
                    btn.innerText = playlist.name;

                    btn.onclick = function () {
                        addSongToPlaylist(playlist.id, songId);
                    };

                    container.appendChild(btn);
                });
            }

            addToPlaylistModal.show();
        })
        .catch(err => console.error("Lỗi khi tải danh sách Playlist:", err));
}

// ===== 2.2 LOGIC THẢ TIM BÀI HÁT (YÊU THÍCH)
function toggleFavoriteCurrentSong() {
    // 1. Kiểm tra xem có bài hát nào đang phát không
    if (!currentPlayingSongId) {
        alert("Vui lòng chọn một bài hát để phát trước!");
        return;
    }

    // 2. Gọi AJAX xuống FavoriteController
    fetch('/Favorite/ToggleFavorite?songId=' + currentPlayingSongId, {
        method: 'POST'
    })
        .then(response => {
            // Nếu server báo lỗi 401 Unauthorized (Chưa đăng nhập)
            if (response.status === 401) {
                window.location.href = '/Auth/Login';
                throw new Error("Chưa đăng nhập");
            }
            return response.json();
        })
        .then(data => {
            if (data && data.success) {
                let heartIcon = document.getElementById('player-heart-icon');

                // 3. Đổi CSS icon dựa trên trạng thái trả về
                if (data.isLiked) {
                    // Đổi thành tim đặc, màu đỏ (hoặc xanh lá tùy theme của bạn)
                    heartIcon.className = "fa-solid fa-heart ms-3 text-danger";
                } else {
                    // Đổi lại thành tim rỗng, màu xám
                    heartIcon.className = "fa-regular fa-heart ms-3 text-secondary";
                }
            } else if (data) {
                alert(data.message);
            }
        })
        .catch(err => console.error("Lỗi khi thả tim:", err));
}


// ===== 2.3 Add Song To Playlist =====
function addSongToPlaylist(playlistId, songId) {

    fetch(`/Playlist/AddSongToPlaylist?playlistId=${playlistId}&songId=${songId}`, {
        method: 'POST'
    })
        .then(response => response.json())
        .then(data => {

            if (data.success) {
                alert(data.message);
                addToPlaylistModal.hide();
            } else {
                alert(data.message);
            }
        })
        .catch(err => console.error("Lỗi khi thêm bài hát:", err));
}



// =====================================================
// SECTION 3: SPA NAVIGATION ENGINE
// =====================================================


// ===== 3.1 Navigate Without Reload =====
function navigateTo(url, pushState = true) {
    fetch(url)
        .then(response => {
            if (!response.ok) throw new Error("Lỗi mạng hoặc server");
            return response.text();
        })
        .then(html => {
            let parser = new DOMParser();
            let doc = parser.parseFromString(html, 'text/html');
            let newContent = doc.querySelector('.content-container');
            let newTitle = doc.querySelector('title');

            if (newContent) {
                document.querySelector('.content-container').innerHTML =
                    newContent.innerHTML;

                if (newTitle) document.title = newTitle.innerText;

                if (pushState) {
                    window.history.pushState({ path: url }, '', url);
                }

                updateSidebarActive(url);

                document.querySelector('.content-container')
                    .querySelectorAll('script')
                    .forEach(oldScript => {
                        const newScript = document.createElement('script');
                        Array.from(oldScript.attributes).forEach(attr =>
                            newScript.setAttribute(attr.name, attr.value)
                        );
                        newScript.textContent = oldScript.textContent;
                        oldScript.parentNode.replaceChild(newScript, oldScript);
                    });

            } else {
                window.location.href = url;
            }
        })
        .catch(error => {
            console.error('Lỗi khi tải trang:', error);
            window.location.href = url;
        });
}

// ===== HELPER: Cập nhật active tab sidebar sau SPA navigate =====
function updateSidebarActive(url) {
    // Xóa active khỏi tất cả nav-item trong sidebar
    document.querySelectorAll('.sidebar .nav-item').forEach(item => {
        item.classList.remove('active');
    });

    // Tìm link khớp với URL hiện tại và set active
    document.querySelectorAll('.sidebar .nav-spa-link').forEach(link => {
        const linkUrl = new URL(link.href, window.location.origin);
        const currentUrl = new URL(url, window.location.origin);

        // So sánh pathname (bỏ qua query string)
        if (linkUrl.pathname === currentUrl.pathname) {
            const navItem = link.querySelector('.nav-item');
            if (navItem) navItem.classList.add('active');
        }
    });
}



// =====================================================
// SECTION 4: DOM READY INITIALIZATION
// =====================================================

document.addEventListener("DOMContentLoaded", function () {

    // ===== 4.1 Init Volume Bar =====
    const volumeBar = document.getElementById("volume-bar");
    if (volumeBar) {
        updateSliderBackground(volumeBar, 100);
    }

    // ===== 4.2 Init Add To Playlist Modal =====
    var modalEl = document.getElementById('addToPlaylistModal');
    if (modalEl) {
        addToPlaylistModal = new bootstrap.Modal(modalEl);
    }

    // ===== 4.3 Intercept Internal Links (SPA) =====
    document.body.addEventListener('click', function (e) {

        let target = e.target.closest('a');

        if (!target || !target.href || target.getAttribute('target') === '_blank') return;
        if (target.getAttribute('href').startsWith('#')) return;

        let url = new URL(target.href);
        if (url.origin !== window.location.origin) return;

        e.preventDefault();
        navigateTo(target.href);
    });

    // ===== 4.4 Browser Back / Forward =====
    window.addEventListener('popstate', function () {
        navigateTo(window.location.href, false);
    });

    // ===== 4.5 Create Playlist AJAX =====
    let formCreatePlaylist = document.getElementById('formCreatePlaylist');

    if (formCreatePlaylist) {

        formCreatePlaylist.addEventListener('submit', function (e) {

            e.preventDefault();
            let formData = new FormData(this);

            fetch('/Playlist/Create', {
                method: 'POST',
                body: formData
            })
                .then(response => response.json())
                .then(data => {

                    if (data.success) {

                        let modalEl = document.getElementById('createPlaylistModal');
                        let modalInstance =
                            bootstrap.Modal.getInstance(modalEl);

                        if (!modalInstance) {
                            modalInstance = new bootstrap.Modal(modalEl);
                        }

                        modalInstance.hide();

                        document.querySelector('.modal-backdrop')?.remove();
                        document.body.classList.remove('modal-open');
                        document.body.style.overflow = '';
                        document.body.style.paddingRight = '';

                        formCreatePlaylist.reset();

                        navigateTo('/Playlist/Detail/' + data.playlistId);
                    } else {
                        alert(data.message);
                    }
                })
                .catch(err => console.error("Lỗi khi tạo playlist:", err));
        });
        // ===== 4. Intercept Internal Links (SPA) =====
        document.body.addEventListener('click', function (e) {

            // --- Xử lý thẻ <a> ---
            let target = e.target.closest('a');
            if (target && target.href && target.getAttribute('target') !== '_blank') {
                if (!target.getAttribute('href').startsWith('#')) {
                    let url = new URL(target.href);
                    if (url.origin === window.location.origin) {
                        e.preventDefault();
                        navigateTo(target.href);
                        return; p
                    }
                }
            }
        });

        document.body.addEventListener('submit', function (e) {
            const form = e.target.closest('form.filter-container');
            if (!form) return; // Không phải form filter → bỏ qua

            e.preventDefault();
            e.stopPropagation();

            // Serialize form thành query string
            const formData = new FormData(form);
            const params = new URLSearchParams(formData).toString();
            const url = form.getAttribute('action') + '?' + params;

            console.log('[SPA Filter] Navigating to:', url); // Debug
            navigateTo(url);
        });
    }

    // ===== 4.6 Search SPA =====
    let headerSearchInput =
        document.getElementById('headerSearchInput');

    if (headerSearchInput) {

        headerSearchInput.addEventListener('keypress', function (e) {

            if (e.key === 'Enter') {

                e.preventDefault();
                let keyword = this.value.trim();

                if (keyword) {
                    navigateTo('/Search?keyword=' +
                        encodeURIComponent(keyword));
                }
            }
        });
    }

});



// ====================================================
// SECTION 5: HỆ THỐNG HÀNG ĐỢI PHÁT NHẠC (QUEUE)
// ====================================================

// THÊM: Logic Bật/Tắt nút Shuffle (Phát ngẫu nhiên)
function toggleShuffle() {
    isShuffle = !isShuffle;
    let btn = document.getElementById("btn-shuffle");

    // Khi bật Shuffle, đổi màu icon sang xanh (hoặc màu nhận diện của web bạn)
    if (isShuffle) {
        btn.style.color = "#1db954";
    } else {
        btn.style.color = ""; // Trở về màu mặc định (xám/trắng)
    }
}

// THÊM: Logic Bật/Tắt nút Repeat (Lặp bài hiện tại)
function toggleRepeat() {
    isRepeat = !isRepeat;
    let btn = document.getElementById("btn-repeat");

    if (isRepeat) {
        btn.style.color = "#1db954";
    } else {
        btn.style.color = "";
    }
}

// THÊM: Xử lý sự kiện khi một bài hát vừa phát xong
function handleSongEnded() {
    // Nếu bật nút lặp bài, gọi lại đúng bài đó (reset cờ tính view lại từ đầu)
    if (isRepeat) {
        playCurrentSongFromQueue();
    } else {
        // Nếu không bật lặp, chuyển sang bài tiếp theo (bên trong nextSong đã xử lý Shuffle)
        nextSong();
    }
}

// Hàm 1: Nhận danh sách bài hát từ input ẩn và bắt đầu phát
function playQueue(queueElementId, startIndex) {
    let queueInput = document.getElementById(queueElementId);

    if (queueInput && queueInput.value) {
        // Biến chuỗi JSON thành mảng dữ liệu thực thụ
        globalQueue = JSON.parse(queueInput.value);
        globalQueueIndex = startIndex;

        playCurrentSongFromQueue();
    } else {
        console.error("Không tìm thấy dữ liệu hàng đợi hoặc danh sách rỗng.");
    }
}

// Hàm 2: Gọi bài hát tại vị trí hiện tại trong mảng để đẩy lên Player
function playCurrentSongFromQueue() {
    if (!globalQueue || globalQueue.length === 0) return;

    let song = globalQueue[globalQueueIndex];

    // Tái sử dụng chính hàm playMusic chuẩn của bạn để phát nhạc và đồng bộ trái tim
    playMusic(song.id, song.audioUrl, song.title, song.artistName, song.imageUrl, song.isVip);
}

// ĐÃ SỬA: Hàm 3: Chuyển sang bài tiếp theo (Bao gồm logic phát ngẫu nhiên)
function nextSong() {
    if (!globalQueue || globalQueue.length === 0) return;

    if (isShuffle) {
        // Nếu bật ngẫu nhiên, random một Index bất kỳ nhưng không được trùng với bài hiện tại
        let randomIndex = globalQueueIndex;
        if (globalQueue.length > 1) { // Chỉ ngẫu nhiên nếu có nhiều hơn 1 bài hát
            while (randomIndex === globalQueueIndex) {
                randomIndex = Math.floor(Math.random() * globalQueue.length);
            }
        }
        globalQueueIndex = randomIndex;
    } else {
        // Nếu không ngẫu nhiên, tuần tự chuyển Index
        globalQueueIndex++;

        // Nếu bấm Next khi đang ở bài cuối cùng, quay vòng lại bài số 1
        if (globalQueueIndex >= globalQueue.length) {
            globalQueueIndex = 0;
        }
    }

    playCurrentSongFromQueue();
}

// Hàm 4: Quay lại bài trước (Previous)
function prevSong() {
    if (!globalQueue || globalQueue.length === 0) return;

    globalQueueIndex--;

    // Nếu bấm Prev khi đang ở bài đầu tiên, vòng xuống bài cuối cùng
    if (globalQueueIndex < 0) {
        globalQueueIndex = globalQueue.length - 1;
    }

    playCurrentSongFromQueue();
}

