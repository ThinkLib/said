import { get } from '../lib/ajax'
import { format } from '../lib/utils'


window.addEventListener('DOMContentLoaded', () => {
  /**
   * 文章列表容器
   */
  const $listBox = document.querySelector('#lists')
  /**
   * 加载更多按钮
   */
  const $more = document.querySelector('#load-more')
  /**
   * "点击加载更多" 按钮容器
   */
  const $a = $more.children[0] as HTMLLinkElement
  // 点击加载更多文本
  const $text = $a.querySelector('span')
  /**
   * "正在加载中" 按钮容器
   */
  const $span = $more.children[1] as HTMLSpanElement

  /**
   * 模板
   */
  const template = document.querySelector('#tmp-item').innerHTML


  /**
   * 最大页数
   */
  const maxPage = parseInt($more.getAttribute('data-max'))


  /**
   * 当前页码
   */
  let currenPage = 1

  $a.addEventListener('click', () => {
    $a.style.display = 'none'
    $span.style.display = ''

    // 发送 ajax
    get({
      url: `/said/get/${currenPage}`,
      callback: (err, data) => {
        if (err) {
          $text.innerHTML = '加载失败，点击重新加载'
          $a.style.display = ''
          $span.style.display = 'none'
          return
        }
        // 有下一页，可以继续加载
        if (currenPage < maxPage) {
          currenPage++
          $a.style.display = ''
        }
        $span.style.display = 'none'

        if (data.lists && data.lists.length) {
          // 填充数据
          const html = data.lists.map(article => {
            return format(template, {
              key: article.key,
              thumb: article.thumb,
              title: article.title,
              songTitle: article.song.title,
              songArtist: article.song.artist,
              songAlbum: article.song.album,
              summary: article.summary,
              localDate: article.info.localDate,
              pv: article.pv,
            })
          }).join('')
          $listBox.insertAdjacentHTML('beforeend', html)
        }
      }
    })
  })
})